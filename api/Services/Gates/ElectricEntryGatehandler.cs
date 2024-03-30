using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.LprData;
using Newtonsoft.Json;

namespace api.Services.Gates
{
    public class ElectricEntryGatehandler : GateHandler
    {
        public ElectricEntryGatehandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }

        public override async Task HandleGateEvent(LprReceiveModel lprReceiveModel)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                lprReceiveModel.vehicleLicense = lprReceiveModel.vehicleLicense.ToUpper().Trim();
                NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(scope);
                UserVehicles? vehicle = normalDataBaseContext.UserVehicles.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense);
                ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);
                Console.WriteLine("vechicle: " + JsonConvert.SerializeObject(vehicle));
                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot not found");
                    return;
                }

                parkingLot.avaiableRegularSpaces++;
                normalDataBaseContext.ParkingLots.Update(parkingLot);
                await normalDataBaseContext.SaveChangesAsync();

                if (vehicle == null)
                {
                    await HandleWalkin(lprReceiveModel, parkingLot);
                    return;
                }

                //this only check if it is Electric space
                Reservations? reservations = GetReservations(normalDataBaseContext, lprReceiveModel, vehicle, SpaceType.ELECTRIC);

                if (reservations != null)
                {
                    await HandleReservation(lprReceiveModel, parkingLot, vehicle, reservations);
                }
                else
                {
                    await HandleWalkin(lprReceiveModel, parkingLot, vehicle);
                }
            }
        }
        protected override async Task HandleWalkin(LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles? vehicles = null)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(scope); ParkingRecords? parkingRecords = normalDataBaseContext.ParkingRecords.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null);

                // Console.WriteLine("Parking Records: " + JsonConvert.SerializeObject(normalDataBaseContext.ParkingRecords.Where(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null && x.spaceType == SpaceType.REGULAR).ToList()));

                if (parkingRecords == null)
                {
                    Console.WriteLine("The vehicle not exit yet");
                    return;
                }
                parkingLot.avaiableElectricSpaces--;
                normalDataBaseContext.ParkingLots.Update(parkingLot);
                parkingRecords.exitTime = DateTime.Now;
                normalDataBaseContext.ParkingRecords.Update(parkingRecords);
                await normalDataBaseContext.SaveChangesAsync();

                //Calculate the last record of the vehicle
                decimal price = await CalculateLastRecord(normalDataBaseContext, parkingLot, SpaceType.REGULAR, parkingRecords);

                Payments lastPayment = normalDataBaseContext.Payments.FirstOrDefault(x => x.paymentID == parkingRecords.paymentID);

                if (lastPayment.paymentStatus == PaymentStatus.Generated)
                {
                    lastPayment.amount = price;
                    lastPayment.paymentStatus = price == 0 ? PaymentStatus.Completed : PaymentStatus.Pending;
                    lastPayment.paymentTime = lastPayment.paymentStatus == PaymentStatus.Completed ? DateTime.Now : null;
                    lastPayment.paymentMethodType = lastPayment.paymentStatus == PaymentStatus.Completed ? PaymentMethodType.Free : null;
                    lastPayment.paymentMethod = lastPayment.paymentStatus == PaymentStatus.Completed ? PaymentMethod.Free : null;
                    normalDataBaseContext.Payments.Update(lastPayment);
                    await normalDataBaseContext.SaveChangesAsync();
                }

                int sessionID = parkingRecords.sessionID;

                CreatePaymentRecord(lprReceiveModel, sessionID, SpaceType.ELECTRIC, vehicles);
            }
        }
        protected override async Task HandleReservation(LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles vehicles, Reservations reservations)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(scope);            //Calculate the last record of the vehicle
                ParkingRecords? parkingRecords = normalDataBaseContext.ParkingRecords.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense && x.exitTime == null);

                parkingRecords.exitTime = DateTime.Now;
                await normalDataBaseContext.SaveChangesAsync();

                decimal price = await CalculateLastRecord(normalDataBaseContext, parkingLot, SpaceType.REGULAR, parkingRecords);

                Payments lastPayment = normalDataBaseContext.Payments.FirstOrDefault(x => x.paymentID == parkingRecords.paymentID);

                if (lastPayment.paymentStatus == PaymentStatus.Generated)
                {
                    lastPayment.amount = price;
                    lastPayment.paymentStatus = price == 0 ? PaymentStatus.Completed : PaymentStatus.Pending;
                    lastPayment.paymentTime = lastPayment.paymentStatus == PaymentStatus.Completed ? DateTime.Now : null;
                    lastPayment.paymentMethodType = lastPayment.paymentStatus == PaymentStatus.Completed ? PaymentMethodType.Free : null;
                    lastPayment.paymentMethod = lastPayment.paymentStatus == PaymentStatus.Completed ? PaymentMethod.Free : null;
                    normalDataBaseContext.Payments.Update(lastPayment);
                    normalDataBaseContext.Payments.Update(lastPayment);
                    await normalDataBaseContext.SaveChangesAsync();
                }

                int sessionID = parkingRecords.sessionID;

                CreatePaymentRecord(lprReceiveModel, sessionID, SpaceType.ELECTRIC, vehicles, reservations);
            }
        }
    }
}