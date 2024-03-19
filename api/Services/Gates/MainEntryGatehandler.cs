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
    public class MainEntryGatehandler : GateHandler
    {
        public MainEntryGatehandler(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory)
        {
        }
        public override async Task HandleGateEvent(LprReceiveModel lprReceiveModel)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(scope); UserVehicles? vehicle = normalDataBaseContext.UserVehicles.FirstOrDefault(x => x.vehicleLicense == lprReceiveModel.vehicleLicense);
                Console.WriteLine("Vehicle: " + vehicle.vehicleID);
                ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == lprReceiveModel.lotID);

                if (parkingLot == null)
                {
                    Console.WriteLine("Parking lot not found");
                    return;
                }

                //not found = not register
                if (vehicle == null)
                {
                    await HandleWalkin(lprReceiveModel, parkingLot);
                    return;
                }

                //check if the vehicle is in reservation even regular or electric
                //allow 5 minutes early and 30 minutes late
                //allow get in even the space if full for walkin
                Reservations? reservations = normalDataBaseContext.Reservations.FirstOrDefault(
                    x => x.vehicleID == vehicle.vehicleID &&
                    x.startTime.AddMinutes(maxEarlyTime) <= DateTime.Now &&
                    x.startTime.AddMinutes(maxLateTime) >= DateTime.Now &&
                    x.reservationStatus == ReservationStatus.PAID &&
                    x.lotID == lprReceiveModel.lotID
                    );
                Console.WriteLine(JsonConvert.SerializeObject(normalDataBaseContext.Reservations.Where(x => x.vehicleID == vehicle.vehicleID).ToList()));

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

        protected override async Task HandleWalkin(LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles? vehicle = null)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(scope); if (parkingLot.avaiableRegularSpaces < parkingLot.reservableOnlyRegularSpaces)
                {
                    Console.WriteLine("Parking lot is full");
                    return;
                }

                parkingLot.avaiableRegularSpaces--;
                await normalDataBaseContext.SaveChangesAsync();

                int sessionID = await createSessionID(normalDataBaseContext, lprReceiveModel);

                CreatePaymentRecord(lprReceiveModel, sessionID, SpaceType.REGULAR, vehicle);

            }
        }

        protected override async Task HandleReservation(LprReceiveModel lprReceiveModel, ParkingLots parkingLot, UserVehicles vehicle, Reservations reservations)
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                NormalDataBaseContext normalDataBaseContext = GetNormalDataBaseContext(scope); if (reservations.spaceType == SpaceType.ELECTRIC)
                {
                    parkingLot.avaiableRegularSpaces--;
                    await normalDataBaseContext.SaveChangesAsync();
                }

                int sessionID = await createSessionID(normalDataBaseContext, lprReceiveModel);

                CreatePaymentRecord(lprReceiveModel, sessionID, SpaceType.REGULAR, vehicle, reservations.spaceType == SpaceType.REGULAR ? reservations : null);
            }
        }
    }
}