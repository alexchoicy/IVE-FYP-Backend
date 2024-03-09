using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Respone
{
    public class PaymentResponseDto
    {
        public int paymentID { get; set; }
        public int userId { get; set; }
        public decimal amount { get; set; }
        public string paymentType { get; set; }
        public int relatedID { get; set; }
        public string paymentMethod { get; set; }
        public string paymentStatus { get; set; }
        public DateTime paymentIssuedAt { get; set; }
    }

    public class DetailedPaymentResponseDto : PaymentResponseDto
    {
        public ReservationResponseDto? reservation { get; set; }
        public ParkingRecordResponseDto? parkingRecord { get; set; }
    }
}