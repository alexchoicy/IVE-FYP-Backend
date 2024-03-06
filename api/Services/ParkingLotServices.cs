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
        ParkingLotReponseDto UpdateRegularParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto);
        ParkingLotReponseDto UpdateElectricParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto);
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
            DateTime now = DateTime.Now;
            DateTime roundedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            IEnumerable<ParkingLotReponseDto> parkingLots = normalDataBaseContext.ParkingLots
                .GroupJoin(
                    normalDataBaseContext.HourlyAvailableSpaces.Where(x => x.dateTime == roundedNow),
                    parkingLot => parkingLot.lotID,
                    availableSpace => availableSpace.lotID,
                    (parkingLot, availableSpaces) => new { parkingLot, availableSpaces })
                .Select(x => new ParkingLotReponseDto
                {
                    lotID = x.parkingLot.lotID,
                    name = x.parkingLot.name,
                    address = x.parkingLot.address,
                    latitude = x.parkingLot.latitude,
                    longitude = x.parkingLot.longitude,
                    totalSpaces = x.parkingLot.totalSpaces,
                    regularSpaces = x.parkingLot.regularSpaces,
                    electricSpaces = x.parkingLot.electricSpaces,
                    regularPlanSpaces = x.parkingLot.regularPlanSpaces,
                    electricPlanSpaces = x.parkingLot.electricPlanSpaces,
                    walkinReservedRatio = x.parkingLot.walkinReservedRatio,
                    reservableOnlyRegularSpaces = x.parkingLot.reservableOnlyRegularSpaces,
                    reservableOnlyElectricSpaces = x.parkingLot.reservableOnlyElectricSpaces,
                    reservedDiscount = x.parkingLot.reservedDiscount,
                    minReservationWindowHours = x.parkingLot.minReservationWindowHours,
                    maxReservationHours = x.parkingLot.maxReservationHours,
                    availableRegularSpaces = x.availableSpaces.FirstOrDefault() != null ? x.availableSpaces.FirstOrDefault().regularSpaceCount : (x.parkingLot.regularSpaces - x.parkingLot.regularPlanSpaces),
                    availableElectricSpaces = x.availableSpaces.FirstOrDefault() != null ? x.availableSpaces.FirstOrDefault().electricSpaceCount : (x.parkingLot.electricSpaces - x.parkingLot.electricPlanSpaces),
                    regularSpacePrices = null,
                    electricSpacePrices = null
                });


            if (parkingLots == null)
            {
                throw new ParkingLotNotFoundException("No parking lots found");
            }
            return parkingLots;
        }

        public ParkingLotReponseDto GetParkingLot(int id)
        {
            DateTime now = DateTime.Now;
            DateTime roundedNow = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            ParkingLots? parkingLot = normalDataBaseContext.ParkingLots.FirstOrDefault(x => x.lotID == id);
            if (parkingLot == null)
            {
                throw new ParkingLotNotFoundException("Parking lot not found");
            }
            HourlyAvailableSpaces? availableSpaces = normalDataBaseContext.HourlyAvailableSpaces.FirstOrDefault(x => x.lotID == id && x.dateTime == roundedNow);
            return new ParkingLotReponseDto
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
                availableRegularSpaces = availableSpaces != null ? availableSpaces.regularSpaceCount : (parkingLot.regularSpaces - parkingLot.regularPlanSpaces),
                availableElectricSpaces = availableSpaces != null ? availableSpaces.electricSpaceCount : (parkingLot.electricSpaces - parkingLot.electricPlanSpaces),
                regularSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.regularSpacePrices),
                electricSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.electricSpacePrices)
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

            normalDataBaseContext.SaveChanges();

            return new ParkingLotReponseDto
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

        public ParkingLotReponseDto UpdateRegularParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto)
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

            return new ParkingLotReponseDto
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
                availableRegularSpaces = parkingLot.regularSpaces - parkingLot.regularPlanSpaces,
                availableElectricSpaces = parkingLot.electricSpaces - parkingLot.electricPlanSpaces,
                regularSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.regularSpacePrices),
                electricSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.electricSpacePrices)
            };
        }
        public ParkingLotReponseDto UpdateElectricParkingLotPrices(int id, IEnumerable<UpdateParkingLotPricesDto> updateParkingLotPricesDto)
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

            return new ParkingLotReponseDto
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
                availableRegularSpaces = parkingLot.regularSpaces - parkingLot.regularPlanSpaces,
                availableElectricSpaces = parkingLot.electricSpaces - parkingLot.electricPlanSpaces,
                regularSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.regularSpacePrices),
                electricSpacePrices = JsonConvert.DeserializeObject<IEnumerable<LotPrices>>(parkingLot.electricSpacePrices)
            };
        }
    }
}