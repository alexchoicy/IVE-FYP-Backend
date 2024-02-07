using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Request;
using api.Models.Respone;
using api.utils;
using static api.Exceptions.AuthException;

namespace api.Services
{
    public interface IAuthServices
    {
        AuthResponeDto? login(LoginRequestDto loginRequestDto);
    }

    public class AuthServices : IAuthServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;

        private readonly JWTServices jwtServices;
        public AuthServices(NormalDataBaseContext normalDataBaseContext, JWTServices jwtServices)
        {
            this.normalDataBaseContext = normalDataBaseContext;
            this.jwtServices = jwtServices;
        }

        public AuthResponeDto? login(LoginRequestDto loginRequestDto)
        {


            Users? user = normalDataBaseContext.users.FirstOrDefault(x => x.userName == loginRequestDto.userName);

            if (user == null)
            {
                throw new UserNotFoundException("The User does not exist");
            }

            if (!user.isActive)
            {
                throw new UserNotActiveException("The User is not active");
            }

            if (user.isLocked && user.lockUntil > DateTime.Now)
            {
                var remainingTime = user.lockUntil.Value - DateTime.Now;
                throw new UserLockedException($"Your account is locked. Please try again in {Math.Round(remainingTime.TotalMinutes)} minutes.");
            }

            if (user.password != loginRequestDto.password)
            {
                user.loginAttempts++;
                if (user.loginAttempts >= 3)
                {
                    user.isLocked = true;
                    user.lockUntil = DateTime.Now.AddMinutes(5);
                }
                normalDataBaseContext.SaveChanges();
                throw new InvalidCredentialsException("The username or password is incorrect");
            }

            if (user.isLocked)
            {
                user.isLocked = false;
                user.lockUntil = null;
                user.loginAttempts = 0;
                normalDataBaseContext.SaveChanges();
            }

            if (user.isLocked)
            {
                if (user.lockUntil > DateTime.Now)
                {
                    return null;
                }
                user.isLocked = false;
                user.lockUntil = null;
                user.loginAttempts = 0;
                normalDataBaseContext.SaveChanges();
            }

            AuthResponeDto response = new AuthResponeDto
            {
                Token = jwtServices.CreateToken(user),
                userName = user.userName,
                email = user.email ?? "you have not set email yet",
                firstName = user.firstName ?? "you have not set first name yet",
                lastName = user.lastName ?? "you have not set last name yet",
                phoneNumber = user.phoneNumber ?? "you have not set phone number yet",
            };

            return response;
        }

    }
}