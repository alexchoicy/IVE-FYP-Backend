using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class MakePaymentRequestDto
    {
        public required string paymentMethod { get; set; }
        public required string paymentMethodType { get; set; }
    }
}