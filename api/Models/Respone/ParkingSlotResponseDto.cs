using api.Models.Entity.NormalDB;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models.Respone
{
    public class ParkingLotResponseDto
    {
        public int LotID { get; set; }
        public required string name { get; set; }
        public required string address { get; set; }
        public required int total { get; set; }
        public required int available { get; set; }
        public List<ParkingSlotResponseDto>? parkingSlots { get; set; }
    }

    public class ParkingSlotResponseDto
    {
        public int SlotID { get; set; }
        public required string SlotType { get; set; }
        public required byte isAvailable { get; set; }
        public required int LotID { get; set; }
    }
}