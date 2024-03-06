using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.LprData
{
    public class LprReceiveModel
    {
        public int lotID { get; set; }
        public required string gateType { get; set; }
        public required string vehicleLicense { get; set; }
    }
}