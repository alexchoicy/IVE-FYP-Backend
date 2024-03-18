using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class ParkingRecords
    {
        [Key]
        public int parkingRecordID { get; set; }
        [ForeignKey("ParkingLots")]
        public required int lotID { get; set; }
        [ForeignKey("Payments")]
        public required int paymentID { get; set; }
        [ForeignKey("Reservations")]
        public int? reservationID { get; set; }
        public required SpaceType spaceType { get; set; }
        public required DateTime entryTime { get; set; }
        public DateTime? exitTime { get; set; }
        public required string vehicleLicense { get; set; }

        [ForeignKey("ParkingRecordSessions")]
        public required int sessionID { get; set; }

        //references
        public virtual Reservations reservation { get; set; }
        public virtual ParkingLots parkingLot { get; set; }
        public virtual Payments payment { get; set; }
    }
}