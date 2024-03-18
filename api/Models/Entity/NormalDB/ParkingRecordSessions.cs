using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entity.NormalDB
{
    public class ParkingRecordSessions
    {
        [Key]
        public int sessionID { get; set; }
        public string vehicleLicense { get; set; }
        [ForeignKey("ParkingLots")]
        public required int lotID { get; set; }
        public decimal? totalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual ParkingLots parkingLot { get; set; }
    }
}