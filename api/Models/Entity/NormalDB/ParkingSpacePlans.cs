using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class ParkingSpacePlans
    {
        [Key]
        public required int spacePlanID { get; set; }
        [ForeignKey("ParkingSpaces")]
        public required int spaceID { get; set; }
        [ForeignKey("ParkingPlans")]
        public required int planID { get; set; }
        [ForeignKey("Users")]
        public required int userID { get; set; }
        [Column(TypeName = "varchar")]
        public required ParkingSpacePlanStatus parkingSpacePlanStatus { get; set; }
        public required DateTime startTime { get; set; }
        public DateTime? endTime { get; set; } //null meaning purchased

        public required DateTime createdAt { get; set; }
        //references
        public required ParkingSpaces space { get; set; }
        public required ParkingPlans plan { get; set; }
        public required Users user { get; set; }
    }
}