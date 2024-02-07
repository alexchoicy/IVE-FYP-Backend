using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Respone;

namespace api.Services
{
    public interface IUserServices
    {
        UserResponeDto userInfo(string userName);
    }
    
    public class UserServices : IUserServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public UserServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public UserResponeDto userInfo(string userID)
        {
            int id = Convert.ToInt32(userID);
            Users? user = normalDataBaseContext.users.FirstOrDefault(x => x.userID == id);
            if (user == null)
            {
                return null;
            }
            return new UserResponeDto
            {
                userID = user.userID,
                userName = user.userName,
                email = user.email,
                firstName = user.firstName,
                lastName = user.lastName,
                phoneNumber = user.phoneNumber,
                createdAt = user.createdAt,
                isActive = user.isActive
            };
        }

    }
}