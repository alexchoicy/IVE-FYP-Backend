using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Models.Entity.NormalDB
{
    public class UserVechicles
    {
        [Key]
        public required int vechicleID { get; set; }
        [ForeignKey("Users")]
        public required int userID { get; set; }
        public required string vechicleLicense { get; set; }
        public VechicleTypes vechicleType { get; set; }
        public required bool isDisabled { get; set; }
        public required DateTime createdAt { get; set; }

        //references
        public required Users user { get; set; }
    }
}