using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public interface ITest
    {
        public Task<IEnumerable<Users>> GetUser();
    }
    public class Test : ITest
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public Test(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }
        public async Task<IEnumerable<Users>> GetUser()
        {
            return await normalDataBaseContext.users.ToListAsync();
        }
    }
}