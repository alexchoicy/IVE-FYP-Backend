using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.LprData;

namespace api.Services.Gates
{
    public class ElectricExitGateHandler : GateHandler
    {
        public ElectricExitGateHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public override void HandleGateEvent(LprReceiveModel lprReceiveModel)
        {
            NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(serviceScopeFactory.CreateScope());
            UserVehicles? vehicle = normalDataBaseContext.UserVehicles.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense);
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);

            if (parkingLot == null)
            {
                Console.WriteLine("Parking lot not found");
                return;
            }

            if (vehicle == null)
            {
                HandleWalkin(normalDataBaseContext, lprReceiveModel, parkingLot);
                return;
            }

            //this only check if it is Electric space
            Reservations? reservations = normalDataBaseContext.Reservations.FirstOrDefault(
                x => x.vehicleID == vehicle.vehicleID &&
                x.startTime.AddMinutes(maxEarlyTime) <= DateTime.Now &&
                x.startTime.AddMinutes(maxLateTime) >= DateTime.Now &&
                x.reservationStatus == ReservationStatus.PAID &&
                x.spaceType == SpaceType.REGULAR &&
                x.lotID == lprReceiveModel.lotID
                );

            if (reservations != null)
            {
                HandleReservation(normalDataBaseContext, lprReceiveModel, parkingLot, vehicle, reservations);
            }
            else
            {
                HandleWalkin(normalDataBaseContext, lprReceiveModel, parkingLot, vehicle);
            }
        }
        protected override void HandleWalkin(NormalDataBaseContext normalDataBaseContext, LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles? vehicles = null)
        {
            ParkingRecords? parkingRecords = normalDataBaseContext.ParkingRecords.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null);

            // Console.WriteLine("Parking Records: " + JsonConvert.SerializeObject(normalDataBaseContext.ParkingRecords.Where(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null && x.spaceType == SpaceType.REGULAR).ToList()));

            if (parkingRecords == null)
            {
                Console.WriteLine("The vehicle not exit yet");
                return;
            }

            parkingRecords.exitTime = DateTime.Now;
            normalDataBaseContext.ParkingRecords.Update(parkingRecords);
            normalDataBaseContext.SaveChanges();

            //Calculate the last record of the vehicle
            decimal price = CalculateLastRecord(normalDataBaseContext, parkingLot, SpaceType.ELECTRIC, parkingRecords);

            Payments lastPayment = normalDataBaseContext.Payments.FirstOrDefault(x => x.paymentID == parkingRecords.paymentID);

            if (lastPayment.paymentStatus == PaymentStatus.Generated)
            {
                lastPayment.amount = price;
                lastPayment.paymentStatus = price == 0 ? PaymentStatus.Completed : PaymentStatus.Pending;
                lastPayment.paymentTime = DateTime.Now;
                normalDataBaseContext.Payments.Update(lastPayment);
                normalDataBaseContext.SaveChanges();
            }

            int sessionID = parkingRecords.sessionID;

            CreatePaymentRecord(normalDataBaseContext, lprReceiveModel, sessionID, SpaceType.REGULAR, vehicles);

        }
        protected override void HandleReservation(NormalDataBaseContext normalDataBaseContext, LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles vehicles, Reservations reservations)
        {
            //Calculate the last record of the vehicle
            ParkingRecords? parkingRecords = normalDataBaseContext.ParkingRecords.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null);

            parkingRecords.exitTime = DateTime.Now;
            normalDataBaseContext.SaveChanges();

            decimal price = CalculateLastRecord(normalDataBaseContext, parkingLot, SpaceType.ELECTRIC, parkingRecords);

            Payments lastPayment = normalDataBaseContext.Payments.FirstOrDefault(x => x.paymentID == parkingRecords.paymentID);

            if (lastPayment.paymentStatus == PaymentStatus.Generated)
            {
                lastPayment.amount = price;
                lastPayment.paymentStatus = price == 0 ? PaymentStatus.Completed : PaymentStatus.Pending;
                lastPayment.paymentTime = DateTime.Now;
                normalDataBaseContext.Payments.Update(lastPayment);
                normalDataBaseContext.SaveChanges();
            }

            int sessionID = parkingRecords.sessionID;

            CreatePaymentRecord(normalDataBaseContext, lprReceiveModel, sessionID, SpaceType.REGULAR, vehicles, reservations);
        }
    }
}