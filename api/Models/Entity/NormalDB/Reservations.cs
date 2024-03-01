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
        public required int reservationID { get; set; }
        [ForeignKey("Uservehicles")]
        public required int vehicleID { get; set; }
        [ForeignKey("ParkingLots")]
        public required int lotID { get; set; }
        public required DateTime startTime { get; set; }
        public required DateTime endTime { get; set; }
        public double price { get; set; }
        public required SpaceType spaceType { get; set; }
        public required ReservationStatus reservationStatus { get; set; }
        public required DateTime CreateAt { get; set; }
        public DateTime? cancelTime { get; set; }
        //references
        public required UserVehicles vehicle { get; set; }
        public required ParkingLots lot { get; set; }
    }
}