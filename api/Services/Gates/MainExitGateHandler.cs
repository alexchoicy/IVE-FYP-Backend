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
    public class MainExitGateHandler : GateHandler
    {
        public MainExitGateHandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public override void HandleGateEvent(LprReceiveModel lprReceiveModel)
        {
            NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(serviceScopeFactory.CreateScope());
            UserVehicles? vehicle = normalDataBaseContext.UserVehicles.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense);
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);
            parkingLot.avaiableRegularSpaces++;
            normalDataBaseContext.SaveChanges();

            HandleFinalExit(normalDataBaseContext, lprReceiveModel, parkingLot, vehicle);
        }

        private void HandleFinalExit(NormalDataBaseContext normalDataBaseContext, LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles? vehicles = null)
        {
            ParkingRecords? parkingRecords = normalDataBaseContext.ParkingRecords.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null);

            if (parkingRecords == null)
            {
                Console.WriteLine("No parking record found");
                return;
            }

            parkingRecords.exitTime = DateTime.Now;
            normalDataBaseContext.ParkingRecords.Update(parkingRecords);
            normalDataBaseContext.SaveChanges();

            decimal price = CalculateLastRecord(normalDataBaseContext, parkingLot, SpaceType.REGULAR, parkingRecords);

            Payments lastPayment = normalDataBaseContext.Payments.FirstOrDefault(x => x.paymentID == parkingRecords.paymentID);

            if (lastPayment.paymentStatus == PaymentStatus.Generated)
            {
                lastPayment.amount = price;
                lastPayment.paymentStatus = price == 0 ? PaymentStatus.Completed : PaymentStatus.Pending;
                lastPayment.paymentTime = DateTime.Now;
                normalDataBaseContext.Payments.Update(lastPayment);
                normalDataBaseContext.SaveChanges();
            }

            if (lastPayment.paymentStatus == PaymentStatus.Completed && lastPayment.paymentTime.Value.AddMinutes(GracePeriodForPayment) < DateTime.Now)
            {
                //TODO: The vehicle has exceeded the grace period for payment, user will be charged for the extra hour
                Console.WriteLine("The vehicle has exceeded the grace period for payment, user will be charged for the extra hour");
                return;
            }

            IEnumerable<ParkingRecords> parkingRecordsList = normalDataBaseContext.ParkingRecords.Where(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.sessionID == parkingRecords.sessionID && x.lotID == parkingRecords.lotID);

            decimal totalAmount = 0;
            decimal unPayedAmount = 0;
            bool isAllCompleted = true;
            foreach (ParkingRecords record in parkingRecordsList)
            {
                if (record.payment.paymentStatus != PaymentStatus.Completed)
                {
                    isAllCompleted = false;
                    unPayedAmount += record.payment.amount;
                }
                totalAmount += record.payment.amount;
            }

            ParkingRecordSessions parkingRecordSessions = normalDataBaseContext.ParkingRecordSessions.FirstOrDefault(x => x.sessionID == parkingRecords.sessionID);
            parkingRecordSessions.totalPrice = totalAmount;
            normalDataBaseContext.ParkingRecordSessions.Update(parkingRecordSessions);
            normalDataBaseContext.SaveChanges();

            if (isAllCompleted)
            {
                Console.WriteLine("No payment required, finished parking session");
                return;
            }

            Console.WriteLine("Total amount: " + totalAmount + " Unpayed amount: " + unPayedAmount);

        }


        protected override void HandleReservation(NormalDataBaseContext normalDataBaseContext, LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles vehicles, Reservations reservations)
        {
            throw new NotImplementedException();
        }

        protected override void HandleWalkin(NormalDataBaseContext normalDataBaseContext, LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles? vehicles = null)
        {
            throw new NotImplementedException();
        }
    }
}