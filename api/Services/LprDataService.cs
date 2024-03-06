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

        private readonly NormalDataBaseContext normalDataBaseContext;

        private readonly IHourlyAvaiableSpaceServices hourlyAvaiableSpaceServices;

        public LprDataService(NormalDataBaseContext normalDataBaseContext, IHourlyAvaiableSpaceServices hourlyAvaiableSpaceServices)
        {
            this.normalDataBaseContext = normalDataBaseContext;
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
            //check if the car is a registered car
            UserVehicles? vehicles = normalDataBaseContext.UserVehicles.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense);
            //if the car is not registered = not a user
            if (vehicles == null)
            {
                HandleWalkin(lprReceiveModel, false);
                return;
            }
            //check if the car is has a reservation
            Reservations? reservations = normalDataBaseContext.Reservations.FirstOrDefault(x => x.vehicleID == vehicles.vehicleID);

            if (reservations == null)
            {
                HandleWalkin(lprReceiveModel, true, vehicles);
                return;
            }

            HandleReservation(lprReceiveModel, vehicles, reservations);
        }

        public async void HandleWalkin(LprReceiveModel lprReceiveModel, bool isUser, UserVehicles? userVehicle = null)
        {
            HourlyAvailableSpaces? hourlyAvailableSpaces = normalDataBaseContext.HourlyAvailableSpaces.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID && x.dateTime == DateTime.Now);
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);

            if (hourlyAvailableSpaces == null)
            {
                hourlyAvailableSpaces = hourlyAvaiableSpaceServices.CreateHourlyAvaiableSpace(parkingLot, DateTime.Now);
                normalDataBaseContext.HourlyAvailableSpaces.Add(hourlyAvailableSpaces);
            }

            if (hourlyAvailableSpaces.regularSpaceCount < parkingLot?.reservableOnlyRegularSpaces)
            {
                Console.WriteLine("Regular Space for walkin is full");
                return;
            }

            //the current design +1 if a car enters Electronic area
            hourlyAvailableSpaces.regularSpaceCount -= 1;

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

            payment.RelatedID = parkingRecords.parkingRecordID;
            await normalDataBaseContext.SaveChangesAsync();

            //TODO: a message will send to somewhere??
        }

        public async void HandleReservation(LprReceiveModel lprReceiveModel, UserVehicles vehicle, Reservations reservation)
        {
            HourlyAvailableSpaces? hourlyAvailableSpaces = normalDataBaseContext.HourlyAvailableSpaces.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID && x.dateTime == DateTime.Now);
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);

            Payments payment = new Payments
            {
                paymentType = PaymentType.ParkingFee,
                userID = vehicle.userID,
            };

            normalDataBaseContext.Payments.Add(payment);
            await normalDataBaseContext.SaveChangesAsync();

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

            payment.RelatedID = parkingRecords.parkingRecordID;
            await normalDataBaseContext.SaveChangesAsync();

            //TODO: a message will send to somewhere??
        }




        // public void CarExit(LprReceiveModel lprReceiveModel)
        // {
        //     ParkingRecords? parkingRecords = normalDataBaseContext.ParkingRecords.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null);
        //     if (parkingRecords == null)
        //     {
        //         Console.WriteLine("Car not found");
        //         return;
        //     }

        //     parkingRecords.exitTime = DateTime.Now;
        //     normalDataBaseContext.ParkingRecords.Update(parkingRecords);

        //     if (parkingRecords.payment.amount == -1)
        //     {
        //         Decimal amount = PaymentUtils.CalculateParkingFee();
        //     }


        //     bool success = Enum.TryParse(parkingRecords.payment.paymentStatus.ToString(), out PaymentStatus paymentStatus);

        //     if (paymentStatus.Equals(PaymentStatus.Pending))
        //     {

        //         //TODO: use machine to pay
        //     }

        //     normalDataBaseContext.SaveChanges();
        // }

        //TODO: later
        public void CarEnterElectronicArea(LprReceiveModel lprReceiveModel)
        {
            Console.WriteLine("Car Enter Electronic");
        }

        public void CarExitElectronicArea(LprReceiveModel lprReceiveModel)
        {
            Console.WriteLine("Car Exit Electronic");
        }
    }
}
