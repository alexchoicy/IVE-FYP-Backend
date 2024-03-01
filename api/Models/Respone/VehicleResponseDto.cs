using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Respone
{
    public class VehicleResponseDto
    {
        public int vehicleID { get; set; }
        public required string vehicleLicense { get; set; }
        public required vehicleTypes vehicleType { get; set; }
        public bool isDisabled { get; set; }
    }
}