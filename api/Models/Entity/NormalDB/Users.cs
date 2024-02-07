using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Entity.NormalDB
{
    public class Users
    {
        [Key]
        public int userID { get; set; }
        public required string userName { get; set; }
        public required string phoneNumber { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string password { get; set; }
        public required string email { get; set; }
        public DateTime createdAt { get; set; }
        public bool isActive { get; set; }
    }
}