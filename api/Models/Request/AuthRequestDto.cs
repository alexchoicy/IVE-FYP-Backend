using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class LoginRequestDto
    {
        public required string username { get; set; }
        public required string password { get; set; }
    }
    public class RegisterRequestDto
    {
        public required string username { get; set; }
        public required string password { get; set; }
    }
    public class ResetPasswordRequestDto
    {
        public required string username { get; set; }
    }

    public class ResetPasswordVeifyRequestDto
    {
        public required string username { get; set; }
        public required string token { get; set; }
        public required string newPassword { get; set; }
    }
}