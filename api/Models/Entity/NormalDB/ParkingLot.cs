using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.Models.Entity.NormalDB
{
    public class ParkingLot
    {
        [Key]
        public int LotID { get; set; }
        public required string name { get; set; }
        public required string address { get; set; }
        public required double lat { get; set; }
        public required double lng { get; set; }
        public required int total { get; set; }
        public required int available { get; set; }

        public List<ParkingSlot>? parkingSlots { get; set; }
    }
}

