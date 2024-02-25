using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;
using Newtonsoft.Json;
namespace api.Services
{
    public interface IParkingLotServices
    {
        IEnumerable<ParkingLotReponseDto>? GetParkingLots();
        ParkingLotReponseDto? GetParkingLot(int id);
        ParkingLotReponseDto UpdateParkingLotInfo(int id, UpdateParkingLotInfoDto updateParkingLotInfoDto);
        ParkingLotReponseDto UpdateParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto);
    }

    public class ParkingLotServices : IParkingLotServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public ParkingLotServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public IEnumerable<ParkingLotReponseDto> GetParkingLots()
        {
            IEnumerable<ParkingLotReponseDto> parkingLots = normalDataBaseContext.ParkingLots.Select
            (x => new ParkingLotReponseDto
            {
                lotID = x.lotID,
                name = x.name,
                address = x.address,
                latitude = x.latitude,
                longitude = x.longitude,
                totalSpaces = x.totalSpaces,
                availableSpaces = x.availableSpaces,
                prices = null
            });
            if (parkingLots == null)
            {
                throw new ParkingLotNotFoundException("No parking lots found");
            }
            return parkingLots;
        }

        public ParkingLotReponseDto GetParkingLot(int id)
        {
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == id);
            if (parkingLot == null)
            {
                throw new ParkingLotNotFoundException("Parking lot not found");
            }
            return new ParkingLotReponseDto
            {
                lotID = parkingLot.lotID,
                name = parkingLot.name,
                address = parkingLot.address,
                latitude = parkingLot.latitude,
                longitude = parkingLot.longitude,
                totalSpaces = parkingLot.totalSpaces,
                availableSpaces = parkingLot.availableSpaces,
                prices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.prices)
            };
        }

        public ParkingLotReponseDto UpdateParkingLotInfo(int id, UpdateParkingLotInfoDto updateParkingLotInfoDto)
        {
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == id);
            if (parkingLot == null)
            {
                throw new ParkingLotNotFoundException("Parking lot not found");
            }
            if (updateParkingLotInfoDto.name != null)
            {
                parkingLot.name = updateParkingLotInfoDto.name;
            }
            if (updateParkingLotInfoDto.address != null)
            {
                parkingLot.address = updateParkingLotInfoDto.address;
            }
            if (updateParkingLotInfoDto.latitude != null)
            {
                parkingLot.latitude = (double)updateParkingLotInfoDto.latitude;
            }
            if (updateParkingLotInfoDto.longitude != null)
            {
                parkingLot.longitude = (double)updateParkingLotInfoDto.longitude;
            }
            if (updateParkingLotInfoDto.totalSpaces != null)
            {
                parkingLot.totalSpaces = (int)updateParkingLotInfoDto.totalSpaces;
            }
            normalDataBaseContext.SaveChanges();

            return new ParkingLotReponseDto
            {
                lotID = parkingLot.lotID,
                name = parkingLot.name,
                address = parkingLot.address,
                latitude = parkingLot.latitude,
                longitude = parkingLot.longitude,
                totalSpaces = parkingLot.totalSpaces,
                availableSpaces = parkingLot.availableSpaces,
                prices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.prices)
            };
        }

        public ParkingLotReponseDto UpdateParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto)
        {

            if (updateParkingLotPricesDto.Count() != 24)
            {
                throw new ParkingLotPriceTimesInvalidException("Invalid number of prices, there should have 24 hours (aka itmes) for each day");
            }

            foreach (UpdateParkingLotPricesDto item in updateParkingLotPricesDto)
            {
                if (!TimeSpan.TryParse(item.time, out TimeSpan time))
                {
                    throw new ParkingLotPriceTimeInvalidException("Invalid time format");
                }

                if (time.TotalHours < 0 || time.TotalHours > 24)
                {
                    throw new ParkingLotPriceTimeInvalidException("Invalid time, time should be between 0 and 24");
                }

                if (item.price < 0)
                {
                    throw new ParkingLotPriceInvalidException("Invalid price, price should be greater than 0");
                }
            }

            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == id);
            if (parkingLot == null)
            {
                throw new ParkingLotNotFoundException("Parking lot not found");
            }

            parkingLot.prices = JsonConvert.SerializeObject(updateParkingLotPricesDto);
            normalDataBaseContext.SaveChanges();

            return new ParkingLotReponseDto
            {
                lotID = parkingLot.lotID,
                name = parkingLot.name,
                address = parkingLot.address,
                latitude = parkingLot.latitude,
                longitude = parkingLot.longitude,
                totalSpaces = parkingLot.totalSpaces,
                availableSpaces = parkingLot.availableSpaces,
                prices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.prices)
            };
        }

    }
}