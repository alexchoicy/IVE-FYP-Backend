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
        AuthResponeDto? register(RegisterRequestDto registerRequestDto);
    }

    public class AuthServices : IAuthServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        private readonly JWTServices jwtServices;
        private readonly HashServices hashServices;
        public AuthServices(NormalDataBaseContext normalDataBaseContext, JWTServices jwtServices, HashServices hashServices)
        {
            this.normalDataBaseContext = normalDataBaseContext;
            this.jwtServices = jwtServices;
            this.hashServices = hashServices;
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

            String hashedPassword = hashServices.HashPassword(loginRequestDto.password, user.slat);

            if (user.password != hashedPassword)
            {
                user.loginAttempts++;
                if (user.loginAttempts >= 3)
                {
                    user.isLocked = true;
                    user.lockUntil = DateTime.Now.AddMinutes(5);
                }
                normalDataBaseContext.SaveChanges();
                throw new InvalidCredentialsException("The password is incorrect");
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
                email = user.email ?? "",
                firstName = user.firstName ?? "",
                lastName = user.lastName ?? "",
                phoneNumber = user.phoneNumber ?? "",
            };

            return response;
        }

        public AuthResponeDto? register(RegisterRequestDto registerRequestDto)
        {
            Users? user = normalDataBaseContext.users.FirstOrDefault(x => x.userName == registerRequestDto.userName);

            if (user != null)
            {
                throw new UserAlreadyExistException("The User already exist");
            }

            String slat = hashServices.slatGenerator();
            String hashedPassword = hashServices.HashPassword(registerRequestDto.password, slat);
            Users newUser = new Users
            {
                userName = registerRequestDto.userName,
                password = hashedPassword,
                slat = slat,
                createdAt = DateTime.Now,
                isActive = true,
                isLocked = false,
                loginAttempts = 0,
            };

            normalDataBaseContext.users.Add(newUser);
            normalDataBaseContext.SaveChanges();

            AuthResponeDto response = new AuthResponeDto
            {
                Token = jwtServices.CreateToken(newUser),
                userName = newUser.userName,
                email = newUser.email ?? "",
                firstName = newUser.firstName ?? "",
                lastName = newUser.lastName ?? "",
                phoneNumber = newUser.phoneNumber ?? "",
            };

            return response;
        }
    }
}