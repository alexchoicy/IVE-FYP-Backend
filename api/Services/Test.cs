using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public interface ITest
    {
        public Task<IEnumerable<User>> GetUser();
    }
    public class Test : ITest
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public Test(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }
        public async Task<IEnumerable<User>> GetUser()
        {
            return await normalDataBaseContext.User.ToListAsync();
        }
    }
}