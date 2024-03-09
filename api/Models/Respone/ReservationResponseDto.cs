using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Respone
{
    public class ReservationResponseDto
    {
        public int reservationID { get; set; }
        public int lotID { get; set; }
        public required string lotName { get; set; }
        public required string spaceType { get; set; }
        public int vehicleID { get; set; }
        public required string vehicleLicense { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public double price { get; set; }
        public ReservationStatus reservationStatus { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime? cancelledTime { get; set; }
    }
}