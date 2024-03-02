using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entity.NormalDB
{
    // public class DailyReservationCount
    // {
    //     [Key]
    //     public required int recordID { get; set; }
    //     public required int lotID { get; set; }
    //     public required DateTime date { get; set; }
    //     public required int count { get; set; }
    // }

    public class HourlyAvailableSpaces
    {
        [Key]
        public int recordID { get; set; }
        public required int lotID { get; set; }
        public required DateTime dateTime { get; set; }
        public required int regularSpaceCount { get; set; }
        public required int electricSpaceCount { get; set; }
    }
}