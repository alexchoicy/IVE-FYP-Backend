using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class UpdateParkingLotInfoDto
    {
        public string? name { get; set; }
        public string? address { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
    }

    public class UpdateParkingLotPricesDto
    {
        public required string time { get; set; }
        public required double price { get; set; }
    }
}