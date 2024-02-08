using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class LoginRequestDto
    {
        public required string userName { get; set; }
        public required string password { get; set; }
    }
    public class RegisterRequestDto
    {
        public required string userName { get; set; }
        public required string password { get; set; }
    }
}