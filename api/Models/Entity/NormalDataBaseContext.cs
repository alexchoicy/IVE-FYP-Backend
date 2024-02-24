using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entity.NormalDB;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    public class NormalDataBaseContext : DbContext
    {
        public NormalDataBaseContext(DbContextOptions<NormalDataBaseContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<ParkingLots> ParkingLots { get; set; }
    }
}