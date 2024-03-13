using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.LprData;
using api.utils;

namespace api.Services
{
    public interface ILprDataService
    {
        void gateManagement(LprReceiveModel lprReceiveModel);
    }
    public class LprDataService : ILprDataService
    {

        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly IHourlyAvaiableSpaceServices hourlyAvaiableSpaceServices;

        public LprDataService(IServiceScopeFactory serviceScopeFactory, IHourlyAvaiableSpaceServices hourlyAvaiableSpaceServices)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.hourlyAvaiableSpaceServices = hourlyAvaiableSpaceServices;
        }

        public void gateManagement(LprReceiveModel lprReceiveModel)
        {
            bool success = Enum.TryParse(lprReceiveModel.gateType, out GateType gateType);

            if (!success)
            {
                return;
            }

            switch (gateType)
            {
                case GateType.IN:
                    Console.WriteLine("IN");
                    CarEnter(lprReceiveModel);
                    break;
                case GateType.OUT:
                    Console.WriteLine("OUT");
                    // CarExit(lprReceiveModel);
                    break;
                case GateType.IN_Electronic:
                    Console.WriteLine("IN_Electronic");
                    CarEnterElectronicArea(lprReceiveModel);
                    break;
                case GateType.OUT_Electronic:
                    Console.WriteLine("OUT_Electronic");
                    CarExitElectronicArea(lprReceiveModel);
                    break;
                default:
                    break;
            }
        }

        public void CarEnter(LprReceiveModel lprReceiveModel)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                //check if the car is a registered car
                UserVehicles? vehicles = normalDataBaseContext.UserVehicles.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense);
                //if the car is not registered = not a user
                if (vehicles == null)
                {
                    HandleWalkin(lprReceiveModel, false);
                    return;
                }
                //check if the car is has a reservation
                Reservations? reservations = normalDataBaseContext.Reservations.FirstOrDefault(x => x.vehicleID == vehicles.vehicleID && x.startTime.Date == DateTime.Now.Date);

                if (reservations == null)
                {
                    HandleWalkin(lprReceiveModel, true, vehicles);
                    return;
                }

                HandleReservation(lprReceiveModel, vehicles, reservations);

            }
        }

        public async void HandleWalkin(LprReceiveModel lprReceiveModel, bool isUser, UserVehicles? userVehicle = null)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                DateTime dateTime = DateTime.Now;
                DateTime roundedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);

                if (parkingLot == null)
                {
                    Console.WriteLine("Parking Lot not found");
                    return;
                }

                //TODO: LPR ERROR
                if (parkingLot.avaiableRegularSpaces < parkingLot?.reservableOnlyRegularSpaces)
                {
                    Console.WriteLine("Regular Space for walkin is full");
                    return;
                }

                //the current design +1 if a car enters Electronic area
                parkingLot.avaiableRegularSpaces -= 1;
                normalDataBaseContext.ParkingLots.Update(parkingLot);
                await normalDataBaseContext.SaveChangesAsync();

                Payments payment = new Payments
                {
                    paymentType = PaymentType.ParkingFee,
                    userID = isUser ? userVehicle.userID : 0,
                };

                normalDataBaseContext.Payments.Add(payment);
                await normalDataBaseContext.SaveChangesAsync();

                ParkingRecords parkingRecords = new ParkingRecords
                {
                    lotID = lprReceiveModel.lotID,
                    paymentID = payment.paymentID,
                    spaceType = SpaceType.REGULAR,
                    entryTime = DateTime.Now,
                    vehicleLicense = lprReceiveModel.vehicleLicense,
                };
                normalDataBaseContext.ParkingRecords.Add(parkingRecords);
                await normalDataBaseContext.SaveChangesAsync();

                payment.relatedID = parkingRecords.parkingRecordID;
                await normalDataBaseContext.SaveChangesAsync();

                //TODO: a message will send to somewhere??
            }
        }

        public async void HandleReservation(LprReceiveModel lprReceiveModel, UserVehicles vehicle, Reservations reservation)
        {

            using (var scope = serviceScopeFactory.CreateScope())
            {
                DateTime dateTime = DateTime.Now;
                DateTime roundedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);

                Payments payment = new Payments
                {
                    paymentType = PaymentType.ParkingFee,
                    userID = vehicle.userID,
                };

                //used to hold the space first avoid if there the driver don't entry electronic area
                //but still be counted as electric space wahahahahahhaha
                if (reservation.spaceType == SpaceType.ELECTRIC)
                {
                    parkingLot.avaiableRegularSpaces -= 1;
                }

                normalDataBaseContext.ParkingLots.Update(parkingLot);
                await normalDataBaseContext.SaveChangesAsync();

                normalDataBaseContext.Payments.Add(payment);
                await normalDataBaseContext.SaveChangesAsync();

                reservation.reservationStatus = ReservationStatus.ACTIVE;
                normalDataBaseContext.Reservations.Update(reservation);

                ParkingRecords parkingRecords = new ParkingRecords
                {
                    lotID = lprReceiveModel.lotID,
                    spaceType = reservation.spaceType,
                    paymentID = payment.paymentID,
                    entryTime = DateTime.Now,
                    reservationID = reservation.reservationID,
                    vehicleLicense = lprReceiveModel.vehicleLicense,
                };

                normalDataBaseContext.ParkingRecords.Add(parkingRecords);
                await normalDataBaseContext.SaveChangesAsync();

                payment.relatedID = parkingRecords.parkingRecordID;
                await normalDataBaseContext.SaveChangesAsync();

                //TODO: a message will send to somewhere??
            }
        }

        public void CarExit(LprReceiveModel lprReceiveModel)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                DateTime dateTime = DateTime.Now;
                DateTime roundedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);

                NormalDataBaseContext normalDataBaseContext = scope.ServiceProvider.GetRequiredService<NormalDataBaseContext>();
                ParkingRecords? parkingRecords = normalDataBaseContext.ParkingRecords.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null);
                ParkingLots parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);
                if (parkingRecords == null)
                {
                    Console.WriteLine("Car not found");
                    return;
                }

                parkingRecords.exitTime = DateTime.Now;
                normalDataBaseContext.ParkingRecords.Update(parkingRecords);
                parkingLot.avaiableRegularSpaces += 1;
                normalDataBaseContext.ParkingLots.Update(parkingLot);

                if (parkingRecords.reservationID != null)
                {
                    Reservations? reservation = normalDataBaseContext.Reservations.Find(parkingRecords.reservationID);
                    if (reservation == null)
                    {
                        Console.WriteLine("Reservation not found");
                        return;
                    }

                    reservation.reservationStatus = ReservationStatus.COMPLETED;

                    normalDataBaseContext.Reservations.Update(reservation);
                }

                normalDataBaseContext.SaveChanges();


                if (parkingRecords.payment.paymentStatus == PaymentStatus.Completed)
                {
                    if (parkingRecords.payment.paymentTime < parkingRecords.payment.paymentTime.Value.AddMinutes(30))
                    {
                        //Finished Payment
                        return;
                    }
                    // TODO: a user leave 30 minutes after payment
                    Console.WriteLine("User leave 30 minutes after payment");
                    return;
                }

                // TODO: use machine to pay
                Console.WriteLine("Use machine to pay");
            }
        }
        //TODO: later
        public void CarEnterElectronicArea(LprReceiveModel lprReceiveModel)
        {
            //if a reservation is regular but entered warn if he don't leave the electronic area in 5 minutes, he will be charged as normal electric space

            Console.WriteLine("Car Enter Electronic");
        }

        public void CarExitElectronicArea(LprReceiveModel lprReceiveModel)
        {
            Console.WriteLine("Car Exit Electronic");
        }
    }
}


// TODO: a cron job that will run every hour to update the hourly available space by (- reservation count)