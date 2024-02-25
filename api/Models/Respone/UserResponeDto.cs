using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models.Respone
{
    public class UserResponeDto
    {
        public int userID { get; set; }
        public required string userName { get; set; }
        public required string phoneNumber { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string email { get; set; }
        public DateTime createdAt { get; set; }
        public bool isActive { get; set; }
    }

    public class StaffReponseDto
    {
        public string? token { get; set; }
        public int userID { get; set; }
        public required string userName { get; set; }
        public required string firstName { get; set; }
        public required string lastName { get; set; }
        public required string email { get; set; }
        public required string phoneNumber { get; set; }
        public int carParkID { get; set; }
    }
}