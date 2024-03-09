using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Respone
{
    public class ParkingRecordResponseDto
    {
        public required int lotID { get; set; }
        public required string lotName { get; set; }
        public required string spaceType { get; set; }
        public required DateTime entryTime { get; set; }
        public DateTime? exitTime { get; set; }
        public required string vehicleLicense { get; set; }
    }

    public class ParkingRecordWithReservationResponseDto : ParkingRecordResponseDto
    {
        public int reservationID { get; set; }
    }
}