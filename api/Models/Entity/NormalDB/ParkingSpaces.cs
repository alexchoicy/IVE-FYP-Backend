using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class ParkingSpaces
    {
        [Key]
        public required int spaceID { get; set; }
        [ForeignKey("ParkingLots")]
        public required int lotID { get; set; }
        public required int floorLevel { get; set; }
        public required int spaceNumber { get; set; }
        [Column(TypeName = "varchar")]
        public required SpaceStatus spaceStatus { get; set; }
        [Column(TypeName = "varchar")]
        public required SpaceType spaceType { get; set; }
        [ForeignKey("ParkingSpacePlans")]
        public int? currentPlanID { get; set; }
        public required bool planEnabled { get; set; }
        public required ParkingLots lot { get; set; }
        public ParkingSpacePlans? currentPlan { get; set; }
    }
}