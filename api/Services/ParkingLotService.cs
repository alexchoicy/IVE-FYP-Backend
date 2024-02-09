using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;
using api.utils;
using Microsoft.EntityFrameworkCore;
using api.Exceptions;

namespace api.Services
{
    public interface IParkingLotService
    {
        Task<IEnumerable<ParkingLot>> getALlParkingLot();
        ParkingLot? getParkingLotByID(int id);
    }

    public class ParkingLotService:IParkingLotService
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public ParkingLotService(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public async Task<IEnumerable<ParkingLot>> getALlParkingLot() 
        {
            return await normalDataBaseContext.parkingLot.ToListAsync(); 
        }

        public ParkingLot? getParkingLotByID(int id)
        {
            if (!normalDataBaseContext.parkingLot.Any(pl => pl.LotID.Equals(id)))
            {
                throw new ParkingLotIDNotExistException("Parking Lot ID not exist");
            }
            ParkingLot? result = normalDataBaseContext.parkingLot.FirstOrDefault(pl => pl.LotID.Equals(id));
            return result;     
        }
    }
}
