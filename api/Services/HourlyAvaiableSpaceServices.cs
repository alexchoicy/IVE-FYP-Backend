using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;
using api.Models;
using api.Models.Entity.NormalDB;

namespace api.Services
{
    public interface IHourlyAvaiableSpaceServices
    {
        HourlyReservationCount CreateHourlyAvaiableSpace(ParkingLots parkingLots, DateTime dateTime);
    }
    public class HourlyAvaiableSpaceServices : IHourlyAvaiableSpaceServices
    {
        // private readonly NormalDataBaseContext normalDataBaseContext;

        // public HourlyAvaiableSpaceServices(NormalDataBaseContext normalDataBaseContext)
        // {
        //     this.normalDataBaseContext = normalDataBaseContext;
        // }

        public HourlyReservationCount CreateHourlyAvaiableSpace(ParkingLots parkingLot, DateTime dateTime)
        {
            DateTime roundedDateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
            HourlyReservationCount newHourlyAvailableSpace = new HourlyReservationCount
            {
                lotID = parkingLot.lotID,
                dateTime = roundedDateTime,
                regularSpaceCount = 0,
                electricSpaceCount = 0,
            };

            return newHourlyAvailableSpace;
        }
    }


}