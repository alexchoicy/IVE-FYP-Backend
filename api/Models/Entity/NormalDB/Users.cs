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
        public string? phoneNumber { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public required string password { get; set; }
        public string? email { get; set; }
        public required DateTime createdAt { get; set; }
        public required bool isActive { get; set; }
    }
}