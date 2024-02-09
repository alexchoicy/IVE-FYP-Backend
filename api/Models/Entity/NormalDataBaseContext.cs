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
        public NormalDataBaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions){}

        public DbSet<Users> users { get; set; }
        public DbSet<ParkingLot> parkingLot { get; set; }
        public DbSet<ParkingSlot> parkingSlot { get; set; }
    }
}