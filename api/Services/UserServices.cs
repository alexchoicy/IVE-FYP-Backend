using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Entity.StaffDB;
using api.Models.Request;
using api.Models.Respone;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public interface IUserServices
    {
        UserResponeDto? getuserInfo(string userID);
        UserResponeDto updateUserInfo(string userID, UserUpdateRequestDto userUpdateRequestDto);
        StaffResponseDto getStaffInfo(string userID);
    }

    public class UserServices : IUserServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        private readonly StaffDataBaseContext staffDataBaseContext;
        public UserServices(NormalDataBaseContext normalDataBaseContext, StaffDataBaseContext staffDataBaseContext)
        {
            this.normalDataBaseContext = normalDataBaseContext;
            this.staffDataBaseContext = staffDataBaseContext;
        }

        public UserResponeDto? getuserInfo(string userID)
        {
            int id = Convert.ToInt32(userID);
            Users? user = normalDataBaseContext.Users.FirstOrDefault(x => x.userID == id);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
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
            try
            {
                int id = Convert.ToInt32(userID);
                Users? user = normalDataBaseContext.Users.FirstOrDefault(x => x.userID == id);
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
            catch (DbUpdateException)
            {
                throw new DataBaseUpdateException("Your Email or Phone number is already used by another user, please try another one");
            }
        }

        public StaffResponseDto getStaffInfo(string userID)
        {
            int id = Convert.ToInt32(userID);
            StaffUsers? user = staffDataBaseContext.users.FirstOrDefault(x => x.UserID == id);
            if (user == null)
            {
                throw new UserNotFoundException("User not found");
            }
            return new StaffResponseDto
            {
                userID = user.UserID,
                userName = user.userName,
                email = user.Email ?? "you have not set email yet",
                firstName = user.FirstName ?? "you have not set first name yet",
                lastName = user.LastName ?? "you have not set last name yet",
                phoneNumber = user.PhoneNumber ?? "you have not set phone number yet",
                carParkID = user.CarParkID
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