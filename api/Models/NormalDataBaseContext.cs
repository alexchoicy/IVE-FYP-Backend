using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    public class NormalDataBaseContext : DbContext
    {
        public NormalDataBaseContext(DbContextOptions dbContextOptions) : base(dbContextOptions){}

        public DbSet<User> User { get; set; } 
    }
}