using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;
using api.utils;
using Microsoft.EntityFrameworkCore;
using api.Exceptions;

namespace api.Services
{
    public interface IParkingSlotService
    {
        ParkingLotResponseDto? getParkingSlotByID(int id);
    }

    public class ParkingSlotService : IParkingSlotService
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public ParkingSlotService(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public ParkingLotResponseDto? getParkingSlotByID(int id)
        {
            var query = from parkingLot in normalDataBaseContext.parkingLot
                        where parkingLot.LotID == id
                        select new ParkingLotResponseDto
                        {
                            LotID = parkingLot.LotID,
                            name = parkingLot.name,
                            address = parkingLot.address,
                            total = parkingLot.total,
                            available = parkingLot.available,
                            parkingSlots = normalDataBaseContext.parkingSlot
                            .Where(ps => ps.LotID == parkingLot.LotID)
                            .Select(ps => new ParkingSlotResponseDto
                            {
                                SlotID = ps.SlotID,
                                SlotType = ps.SlotType,
                                isAvailable = ps.isAvailable,
                                LotID = ps.LotID
                            }).ToList()
                        };

            ParkingLotResponseDto? result = query.FirstOrDefault();
            return result;
        }
    }
}
