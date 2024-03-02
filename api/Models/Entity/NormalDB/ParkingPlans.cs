using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class ParkingPlans
    {
        [Key]
        public required int planID { get; set; }
        public required string name { get; set; }
        [Column(TypeName = "varchar")]
        public required PlanType planType { get; set; }
        public required string description { get; set; }
        public required double price { get; set; }
        public required int durationMonths { get; set; }
    }
}