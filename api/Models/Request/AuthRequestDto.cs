using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Request
{
    public class LoginRequestDto
    {
        public string userName { get; set; }
        public string password { get; set; }
    }    
}