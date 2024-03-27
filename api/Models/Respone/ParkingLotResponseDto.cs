using api.Models.Entity.NormalDB;

namespace api.Models.Respone
{
    public class ParkingLotResponseDto
    {
        public int lotID { get; set; }
        public required string name { get; set; }
        public required string address { get; set; }
        public required string district { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int totalSpaces { get; set; }
        public int regularSpaces { get; set; }
        public int electricSpaces { get; set; }
        public int regularPlanSpaces { get; set; }
        public int electricPlanSpaces { get; set; }
        public decimal walkinReservedRatio { get; set; }
        public int reservableOnlyRegularSpaces { get; set; }
        public int reservableOnlyElectricSpaces { get; set; }
        public decimal reservedDiscount { get; set; }
        public int minReservationWindowHours { get; set; }
        public int maxReservationHours { get; set; }

        public int availableRegularSpaces { get; set; }
        public int availableElectricSpaces { get; set; }
        public IEnumerable<LotPrices>? regularSpacePrices { get; set; }
        public IEnumerable<LotPrices>? electricSpacePrices { get; set; }
    }
}