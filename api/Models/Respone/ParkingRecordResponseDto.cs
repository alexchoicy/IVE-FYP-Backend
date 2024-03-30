using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Respone
{
    public class ParkingRecordResponseDto
    {
        public required int recordID { get; set; }
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

    public class ParkingRecordResponseDtoDetailed
    {
        public required int sessionID { get; set; }
        public required int lotID { get; set; }
        public required string lotName { get; set; }
        public required string vehicleLicense { get; set; }
        public required DateTime entryTime { get; set; }
        public DateTime? exitTime { get; set; }
        public decimal? totalPrice { get; set; }
        public required ICollection<ParkingRecordResponseDtoDetailedHistory> records { get; set; }
    }

    public class ParkingRecordResponseDtoDetailedHistory
    {
        public required int parkingRecordID { get; set; }
        public required double period { get; set; }
        public required DateTime entryTime { get; set; }
        public DateTime? exitTime { get; set; }
        public ReservationResponseDto? reservation { get; set; }
        public required decimal? price { get; set; }
        public required string spaceType { get; set; }
        public required string paymentStatus { get; set; }
    }
}