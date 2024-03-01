using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class UserVehicles
    {
        [Key]
        public int vehicleID { get; set; }
        [ForeignKey("Users")]
        public required int userID { get; set; }
        public required string vehicleLicense { get; set; }
        public VehicleTypes vehicleType { get; set; }
        public required bool isDisabled { get; set; }
        public required DateTime createdAt { get; set; }

        //references
        public virtual Users user { get; set; }
    }
}