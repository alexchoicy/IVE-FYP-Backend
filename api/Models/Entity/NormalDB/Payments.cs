using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entity.NormalDB
{
    public class Payments
    {
        [Key]
        public int paymentID { get; set; }
        public required bool isPaid { get; set; } = false;
        public double? amount { get; set; }
        public DateTime? paymentTime { get; set; }
    }
}