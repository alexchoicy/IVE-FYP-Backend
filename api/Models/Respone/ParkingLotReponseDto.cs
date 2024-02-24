using api.Models.Entity.NormalDB;

namespace api.Models.Respone
{
    public class ParkingLotReponseDto
    {
        public int lotID { get; set; }
        public required string name { get; set; }
        public required string address { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int totalSpaces { get; set; }
        public int availableSpaces { get; set; }
        public required IEnumerable<LotPrices>? prices { get; set; }
    }
}