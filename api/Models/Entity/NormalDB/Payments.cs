using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class Payments
    {
        [Key]
        public int paymentID { get; set; }
        public required int userID { get; set; } // 0 if not a user
        public PaymentStatus paymentStatus { get; set; } = PaymentStatus.Pending;
        public DateTime? paymentTime { get; set; } = null;
        public decimal amount { get; set; } = -1;
        public PaymentMethod paymentMethod { get; set; }
        public PaymentMethodType paymentMethodType { get; set; }
        public PaymentType paymentType { get; set; }
        public int RelatedID { get; set; }

    }
}