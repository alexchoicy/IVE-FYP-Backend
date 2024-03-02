using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class Reservations
    {
        [Key]
        public int reservationID { get; set; }
        [ForeignKey("Uservehicles")]
        public required int vehicleID { get; set; }
        [ForeignKey("ParkingLots")]
        public required int lotID { get; set; }
        public required DateTime startTime { get; set; }
        public required DateTime endTime { get; set; }
        public double price { get; set; }
        [Column(TypeName = "varchar")]
        public required SpaceType spaceType { get; set; }
        [Column(TypeName = "varchar")]
        public required ReservationStatus reservationStatus { get; set; }
        public required DateTime createdAt { get; set; }
        public DateTime? canceledAt { get; set; }
        //references
        public virtual UserVehicles vehicle { get; set; }
        public virtual ParkingLots lot { get; set; }
    }
}