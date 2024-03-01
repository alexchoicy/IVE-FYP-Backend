using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entity.NormalDB
{
    public class ParkingRecords
    {
        [Key]
        public required int parkingRecordID { get; set; }
        [ForeignKey("ParkingLots")]
        public required int lotID { get; set; }
        [ForeignKey("Payments")]
        public required int paymentID { get; set; }
        public required DateTime entryTime { get; set; }
        public DateTime? exitTime { get; set; }
        [ForeignKey("Reservations")]
        public int? reservationID { get; set; }
        public required string vehicleLicense { get; set; }

        //references
        public Reservations? reservation { get; set; }
        public required Payments payment { get; set; }
    }
}