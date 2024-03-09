using System.ComponentModel.DataAnnotations;


namespace api.Models.Entity.NormalDB
{
    public class ParkingLots
    {
        [Key]
        public required int lotID { get; set; }
        public required string name { get; set; }
        public required string address { get; set; }
        public required double latitude { get; set; }
        public required double longitude { get; set; }
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
        public required string regularSpacePrices { get; set; }
        public required string electricSpacePrices { get; set; }
    }
    //the lots will store the prices as a string in the format of a json array
    public class LotPrices
    {
        public required string time { get; set; }
        public required decimal price { get; set; }
    }
}