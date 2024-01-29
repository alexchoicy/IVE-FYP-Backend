using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entity
{
    public class User
    {
        public int UserID { get; set; }
        public int PhoneNumber { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}