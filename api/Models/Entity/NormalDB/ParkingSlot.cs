using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Entity.NormalDB
{
    public class ParkingSlot
    {
        [Key]
        public int SlotID { get; set; }
        
        public required string SlotType { get; set; }
        public required byte isAvailable { get; set; }

        [ForeignKey("ParkingLot")]
        public required int LotID { get; set; }
    }
}