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
        IEnumerable<ParkingLotResponseDto>? GetParkingLots();
        ParkingLotResponseDto? GetParkingLot(int id);
        ParkingLotResponseDto UpdateParkingLotInfo(int id, UpdateParkingLotInfoDto updateParkingLotInfoDto);
        ParkingLotResponseDto UpdateRegularParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto);
        ParkingLotResponseDto UpdateElectricParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto);
    }

    public class ParkingLotServices : IParkingLotServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public ParkingLotServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public IEnumerable<ParkingLotResponseDto> GetParkingLots()
        {
            DateTime now = DateTime.Now;
            DateTime roundedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            IEnumerable<ParkingLotResponseDto> parkingLots = normalDataBaseContext.ParkingLots.Select(x => new ParkingLotResponseDto
            {
                lotID = x.lotID,
                name = x.name,
                address = x.address,
                latitude = x.latitude,
                longitude = x.longitude,
                totalSpaces = x.totalSpaces,
                regularSpaces = x.regularSpaces,
                electricSpaces = x.electricSpaces,
                regularPlanSpaces = x.regularPlanSpaces,
                electricPlanSpaces = x.electricPlanSpaces,
                walkinReservedRatio = x.walkinReservedRatio,
                reservableOnlyRegularSpaces = x.reservableOnlyRegularSpaces,
                reservableOnlyElectricSpaces = x.reservableOnlyElectricSpaces,
                reservedDiscount = x.reservedDiscount,
                minReservationWindowHours = x.minReservationWindowHours,
                maxReservationHours = x.maxReservationHours,
                availableRegularSpaces = x.avaiableRegularSpaces,
                availableElectricSpaces = x.avaiableElectricSpaces,
                regularSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(x.regularSpacePrices),
                electricSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(x.electricSpacePrices)
            }).ToList();


            if (parkingLots == null)
            {
                throw new ParkingLotNotFoundException("No parking lots found");
            }
            return parkingLots;
        }

        public ParkingLotResponseDto GetParkingLot(int id)
        {
            DateTime now = DateTime.Now;
            DateTime roundedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == id);
            if (parkingLot == null)
            {
                throw new ParkingLotNotFoundException("Parking lot not found");
            }
            return new ParkingLotResponseDto
            {
                lotID = parkingLot.lotID,
                name = parkingLot.name,
                address = parkingLot.address,
                latitude = parkingLot.latitude,
                longitude = parkingLot.longitude,
                totalSpaces = parkingLot.totalSpaces,
                regularSpaces = parkingLot.regularSpaces,
                electricSpaces = parkingLot.electricSpaces,
                regularPlanSpaces = parkingLot.regularPlanSpaces,
                electricPlanSpaces = parkingLot.electricPlanSpaces,
                walkinReservedRatio = parkingLot.walkinReservedRatio,
                reservableOnlyRegularSpaces = parkingLot.reservableOnlyRegularSpaces,
                reservableOnlyElectricSpaces = parkingLot.reservableOnlyElectricSpaces,
                reservedDiscount = parkingLot.reservedDiscount,
                minReservationWindowHours = parkingLot.minReservationWindowHours,
                maxReservationHours = parkingLot.maxReservationHours,
                availableRegularSpaces = parkingLot.avaiableRegularSpaces,
                availableElectricSpaces = parkingLot.avaiableElectricSpaces,
                regularSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.regularSpacePrices),
                electricSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.electricSpacePrices)
            };
        }

        public ParkingLotResponseDto UpdateParkingLotInfo(int id, UpdateParkingLotInfoDto updateParkingLotInfoDto)
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

            normalDataBaseContext.SaveChanges();

            return new ParkingLotResponseDto
            {
                lotID = parkingLot.lotID,
                name = parkingLot.name,
                address = parkingLot.address,
                latitude = parkingLot.latitude,
                longitude = parkingLot.longitude,

                regularPlanSpaces = parkingLot.regularPlanSpaces,
                electricPlanSpaces = parkingLot.electricPlanSpaces,
            };
        }

        public ParkingLotResponseDto UpdateRegularParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto)
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

            parkingLot.regularSpacePrices = JsonConvert.SerializeObject(updateParkingLotPricesDto);
            normalDataBaseContext.SaveChanges();

            return new ParkingLotResponseDto
            {
                lotID = parkingLot.lotID,
                name = parkingLot.name,
                address = parkingLot.address,
                latitude = parkingLot.latitude,
                longitude = parkingLot.longitude,
                totalSpaces = parkingLot.totalSpaces,
                regularSpaces = parkingLot.regularSpaces,
                electricSpaces = parkingLot.electricSpaces,
                regularPlanSpaces = parkingLot.regularPlanSpaces,
                electricPlanSpaces = parkingLot.electricPlanSpaces,
                walkinReservedRatio = parkingLot.walkinReservedRatio,
                reservableOnlyRegularSpaces = parkingLot.reservableOnlyRegularSpaces,
                reservableOnlyElectricSpaces = parkingLot.reservableOnlyElectricSpaces,
                reservedDiscount = parkingLot.reservedDiscount,
                minReservationWindowHours = parkingLot.minReservationWindowHours,
                maxReservationHours = parkingLot.maxReservationHours,
                availableRegularSpaces = parkingLot.avaiableRegularSpaces,
                availableElectricSpaces = parkingLot.avaiableElectricSpaces,
                regularSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.regularSpacePrices),
                electricSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.electricSpacePrices)
            };
        }
        public ParkingLotResponseDto UpdateElectricParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto)
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

            parkingLot.electricSpacePrices = JsonConvert.SerializeObject(updateParkingLotPricesDto);
            normalDataBaseContext.SaveChanges();

            return new ParkingLotResponseDto
            {
                lotID = parkingLot.lotID,
                name = parkingLot.name,
                address = parkingLot.address,
                latitude = parkingLot.latitude,
                longitude = parkingLot.longitude,
                totalSpaces = parkingLot.totalSpaces,
                regularSpaces = parkingLot.regularSpaces,
                electricSpaces = parkingLot.electricSpaces,
                regularPlanSpaces = parkingLot.regularPlanSpaces,
                electricPlanSpaces = parkingLot.electricPlanSpaces,
                walkinReservedRatio = parkingLot.walkinReservedRatio,
                reservableOnlyRegularSpaces = parkingLot.reservableOnlyRegularSpaces,
                reservableOnlyElectricSpaces = parkingLot.reservableOnlyElectricSpaces,
                reservedDiscount = parkingLot.reservedDiscount,
                minReservationWindowHours = parkingLot.minReservationWindowHours,
                maxReservationHours = parkingLot.maxReservationHours,
                availableRegularSpaces = parkingLot.avaiableRegularSpaces,
                availableElectricSpaces = parkingLot.avaiableElectricSpaces,
                regularSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.regularSpacePrices),
                electricSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.electricSpacePrices)
            };
        }
    }
}