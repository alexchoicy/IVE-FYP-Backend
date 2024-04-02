using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Request
{
    public class CreateReservationRequestDto
    {
        public required int vehicleID { get; set; }
        public required int lotID { get; set; }
        public required DateTime startTime { get; set; }
        public required DateTime endTime { get; set; }
        public required string spaceType { get; set; }
    }
    public class EditReservationRequestDto
    {
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string spaceType { get; set; }
    }
}