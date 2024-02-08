using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;

namespace api.Services
{
    public interface IUserServices
    {
        UserResponeDto? getuserInfo(string userID);
        UserResponeDto updateUserInfo(string userID, UserUpdateRequestDto userUpdateRequestDto);
    }

    public class UserServices : IUserServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        public UserServices(NormalDataBaseContext normalDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
        }

        public UserResponeDto? getuserInfo(string userID)
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

        public UserResponeDto updateUserInfo(string userID, UserUpdateRequestDto userUpdateRequestDto)
        {
            int id = Convert.ToInt32(userID);
            Users? user = normalDataBaseContext.users.FirstOrDefault(x => x.userID == id);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }

            if (userUpdateRequestDto.email != null)
            {
                if (!IsValidEmail(userUpdateRequestDto.email))
                {
                    throw new InvalidEmailException("Invalid email");
                }
                user.email = userUpdateRequestDto.email;
            }

            if (userUpdateRequestDto.firstName != null)
            {
                user.firstName = userUpdateRequestDto.firstName;
            }

            if (userUpdateRequestDto.lastName != null)
            {
                user.lastName = userUpdateRequestDto.lastName;
            }

            if (userUpdateRequestDto.phoneNumber != null)
            {
                if (!IsValidPhoneNumber(userUpdateRequestDto.phoneNumber))
                {
                    throw new InvalidPhoneNumberException("Invalid phone number");
                }
                user.phoneNumber = userUpdateRequestDto.phoneNumber;
            }

            normalDataBaseContext.SaveChanges();
            
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

        public static bool IsValidEmail(string email)
        {
            string emailRegex = @"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$";
            return Regex.IsMatch(email, emailRegex);
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            string phoneRegex = @"^[2-9]\d{7}$";
            return Regex.IsMatch(phoneNumber, phoneRegex);
        }
        
    }
}