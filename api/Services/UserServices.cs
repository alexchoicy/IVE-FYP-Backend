using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Respone;

namespace api.Services
{
    public interface IUserServices
    {
        UserResponeDto? userInfo(string userName);
        bool IsUserExists(string userName);
        bool isUserActive(string userName);
        bool IsUserLockedOut(string userName);
        DateTime GetLockoutEndDate(string userName);
    }

    public class UserServices : IUserServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public UserServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public UserResponeDto? userInfo(string userID)
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
                email = user.email ?? "you have not set email yet",
                firstName = user.firstName ?? "you have not set first name yet",
                lastName = user.lastName ?? "you have not set last name yet",
                phoneNumber = user.phoneNumber ?? "you have not set phone number yet",
                createdAt = user.createdAt,
                isActive = user.isActive
            };
        }

        public bool IsUserExists(string userName)
        {
            return normalDataBaseContext.users.Any(x => x.userName == userName);
        }

        public bool isUserActive(string userName)
        {
            return normalDataBaseContext.users.Any(x => x.userName == userName && x.isActive);
        }

        public bool IsUserLockedOut(string userName)
        {
            var user = normalDataBaseContext.users.FirstOrDefault(x => x.userName == userName);
            return user != null && user.lockUntil > DateTime.Now;
        }

        public DateTime GetLockoutEndDate(string userName)
        {
            var user = normalDataBaseContext.users.FirstOrDefault(x => x.userName == userName);
            return user?.lockUntil ?? DateTime.Now;
        }
    }
}