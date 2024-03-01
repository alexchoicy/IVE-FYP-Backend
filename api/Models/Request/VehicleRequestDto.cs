using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class VehicleRequestDto
    {
        public string vehicleLicense { get; set; }
        public int vehicleType { get; set; }
    }
}