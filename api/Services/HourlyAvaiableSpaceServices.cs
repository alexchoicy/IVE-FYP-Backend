using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;

namespace api.Services
{
    public interface IHourlyAvaiableSpaceServices
    {
        HourlyAvailableSpaces CreateHourlyAvaiableSpace(ParkingLots parkingLots, DateTime dateTime);
    }
    public class HourlyAvaiableSpaceServices : IHourlyAvaiableSpaceServices
    {
        // private readonly NormalDataBaseContext normalDataBaseContext;

        // public HourlyAvaiableSpaceServices(NormalDataBaseContext normalDataBaseContext)
        // {
        //     this.normalDataBaseContext = normalDataBaseContext;
        // }

        public HourlyAvailableSpaces CreateHourlyAvaiableSpace(ParkingLots parkingLot, DateTime dateTime)
        {
            DateTime roundedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
            HourlyAvailableSpaces newHourlyAvailableSpace = new HourlyAvailableSpaces
            {
                lotID = parkingLot.lotID,
                dateTime = roundedDateTime,
                regularSpaceCount = parkingLot.regularSpaces - parkingLot.regularPlanSpaces,
                electricSpaceCount = parkingLot.electricSpaces - parkingLot.electricPlanSpaces,
            };

            return newHourlyAvailableSpace;
        }
    }


}