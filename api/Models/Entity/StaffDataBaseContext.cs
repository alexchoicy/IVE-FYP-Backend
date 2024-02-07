using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entity.StaffDB;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    public class StaffDataBaseContext : DbContext
    {
        public StaffDataBaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions){}
        public DbSet<Users> users { get; set; } 

    }
}