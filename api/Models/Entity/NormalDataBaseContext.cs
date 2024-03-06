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
        public DbSet<UserVehicles> UserVehicles { get; set; }
        public DbSet<ParkingLots> ParkingLots { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
        public DbSet<HourlyAvailableSpaces> HourlyAvailableSpaces { get; set; }

        public DbSet<ParkingRecords> ParkingRecords { get; set; }
        public DbSet<Payments> Payments { get; set; }
    }
}