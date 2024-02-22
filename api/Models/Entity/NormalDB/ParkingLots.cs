using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        public required int totalSpaces { get; set; }
        public required int availableSpaces { get; set; }
        public required string prices { get; set; }
    }
    //the lots will store the prices as a string in the format of a json array
    public class LotPrices
    {
        public required string time { get; set; }
        public required double price { get; set; }
    }
}