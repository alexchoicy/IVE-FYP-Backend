using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class UserUpdateRequestDto
    {
        public string? email { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? phoneNumber { get; set; }
    }
    
}