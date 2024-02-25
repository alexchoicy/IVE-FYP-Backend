using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Respone
{
    public class AuthResponeDto
    {
        public string? token { get; set; }
        public required string userName { get; set; }
        public required string phoneNumber { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string email { get; set; }
    }
}