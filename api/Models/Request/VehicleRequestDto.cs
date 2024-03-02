using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Request
{
    public class VehicleRequestDto
    {
        public string vehicleLicense { get; set; }
        public string vehicleType { get; set; }
    }
}