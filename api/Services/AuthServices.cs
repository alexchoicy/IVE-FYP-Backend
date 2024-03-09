using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using api.Exceptions;
using api.Models;
using api.Models.Entity.NormalDB;
using api.Models.Entity.StaffDB;
using api.Models.Request;
using api.Models.Respone;
using api.utils;

namespace api.Services
{
    public interface IAuthServices
    {
        AuthResponeDto? login(LoginRequestDto loginRequestDto);
        AuthResponeDto? register(RegisterRequestDto registerRequestDto);
        bool resetPassword(ResetPasswordRequestDto resetPasswordRequestDto);
        bool resetPasswordVeify(ResetPasswordVeifyRequestDto resetPasswordVeifyRequestDto);
        (StaffResponseDto?, string) AdminLogin(LoginRequestDto loginRequestDto);
    }

    public class AuthServices : IAuthServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;
        private readonly StaffDataBaseContext staffDataBaseContext;
        private readonly JWTServices jwtServices;
        private readonly HashServices hashServices;
        private readonly IConfiguration config;
        public AuthServices(NormalDataBaseContext normalDataBaseContext, StaffDataBaseContext staffDataBaseContext, JWTServices jwtServices, HashServices hashServices, IConfiguration config)
        {
            this.normalDataBaseContext = normalDataBaseContext;
            this.staffDataBaseContext = staffDataBaseContext;
            this.jwtServices = jwtServices;
            this.hashServices = hashServices;
            this.config = config;
        }

        public AuthResponeDto? login(LoginRequestDto loginRequestDto)
        {


            Users? user = normalDataBaseContext.Users.FirstOrDefault(x => x.userName == loginRequestDto.username);

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
                TimeSpan remainingTime = user.lockUntil.Value - DateTime.Now;
                throw new UserLockedException($"Your account is locked. Please try again in {Math.Round(remainingTime.TotalMinutes)} minutes.");
            }

            String hashedPassword = hashServices.HashPassword(loginRequestDto.password, user.salt);

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
                token = jwtServices.CreateToken(user),
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
            Users? user = normalDataBaseContext.Users.FirstOrDefault(x => x.userName == registerRequestDto.username);

            if (user != null)
            {
                throw new UserAlreadyExistException("The User already exist");
            }

            if (!isValidPassword(registerRequestDto.password))
            {
                throw new InvalidCredentialsException("The password is not strong enough, please make your password longer than 8 characters, and include at least one number, one uppercase letter, and one lowercase letter.");
            }

            String salt = hashServices.saltGenerator();
            String hashedPassword = hashServices.HashPassword(registerRequestDto.password, salt);
            Users newUser = new Users
            {
                userName = registerRequestDto.username,
                password = hashedPassword,
                salt = salt,
                createdAt = DateTime.Now,
                isActive = true,
                isLocked = false,
                loginAttempts = 0,
            };

            normalDataBaseContext.Users.Add(newUser);
            normalDataBaseContext.SaveChanges();

            AuthResponeDto response = new AuthResponeDto
            {
                token = jwtServices.CreateToken(newUser),
                userName = newUser.userName,
                email = newUser.email ?? "",
                firstName = newUser.firstName ?? "",
                lastName = newUser.lastName ?? "",
                phoneNumber = newUser.phoneNumber ?? "",
            };

            return response;
        }
        public bool resetPassword(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            Users? user = normalDataBaseContext.Users.FirstOrDefault(x => x.userName == resetPasswordRequestDto.username);

            if (user == null)
            {
                throw new UserNotFoundException("The User does not exist");
            }

            string token = jwtServices.CreateTokenForReset(user);

            string url = config.GetValue<string>("ResetPasswordUrl") ?? "";
            if (url == "")
            {
                throw new Exception("ResetPasswordUrl is not found in appsettings.json");
            }
            var data = new
            {
                content = token
            };

            string result = httpRequest.MakePostRequest(url, data).Result;
            if (result == "Success")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool resetPasswordVeify(ResetPasswordVeifyRequestDto resetPasswordVeifyRequestDto)
        {
            string userid = JWTServicesExtension.getUserIDByToken(resetPasswordVeifyRequestDto.token) ?? "";
            if (userid == "")
            {
                throw new InvalidCredentialsException("The token is invalid");
            }

            string type = JWTServicesExtension.getTypeByToken(resetPasswordVeifyRequestDto.token) ?? "";

            if (type != "password-reset")
            {
                throw new InvalidCredentialsException("The token Type incorrect");
            }

            string time = JWTServicesExtension.getExpireTimeByToken(resetPasswordVeifyRequestDto.token) ?? "";

            if (time == "")
            {
                throw new InvalidCredentialsException("The token is invalid");
            }

            if (DateTimeOffset.FromUnixTimeSeconds(long.Parse(time)) < DateTimeOffset.Now)
            {
                throw new InvalidCredentialsException("The token is expired");
            }

            if (!isValidPassword(resetPasswordVeifyRequestDto.newPassword))
            {
                throw new InvalidCredentialsException("The password is not strong enough, please make your password longer than 8 characters, and include at least one number, one uppercase letter, and one lowercase letter.");
            }

            Users? user = normalDataBaseContext.Users.FirstOrDefault(x => x.userID == int.Parse(userid));

            if (user == null)
            {
                throw new UserNotFoundException("The User does not exist");
            }

            string salt = hashServices.saltGenerator();
            string hashedPassword = hashServices.HashPassword(resetPasswordVeifyRequestDto.newPassword, salt);

            user.salt = salt;
            user.password = hashedPassword;
            normalDataBaseContext.SaveChanges();

            return true;
        }

        public bool isValidPassword(string password, int minLegth = 8)
        {

            var hasNumberRegex = @"[0-8]+";
            var hasUpperCharRegex = @"[A-Z]+";
            var hasLowerCharRegex = @"[a-z]+";

            bool isValid =
                password.Length >= minLegth &&
                Regex.IsMatch(password, hasNumberRegex) &&
                Regex.IsMatch(password, hasUpperCharRegex) &&
                Regex.IsMatch(password, hasLowerCharRegex);

            return isValid;
        }


        public (StaffResponseDto, string) AdminLogin(LoginRequestDto loginRequestDto)
        {
            StaffUsers? user = staffDataBaseContext.users.FirstOrDefault(x => x.userName == loginRequestDto.username);

            if (user == null)
            {
                throw new UserNotFoundException("The User does not exist");
            }

            string token = jwtServices.CreateAdminToken(user);
            StaffResponseDto response = new StaffResponseDto
            {
                userName = user.userName,
                email = user.Email ?? "",
                firstName = user.FirstName ?? "",
                lastName = user.LastName ?? "",
                phoneNumber = user.PhoneNumber ?? "",
                carParkID = user.CarParkID
            };
            return (response, token);
        }
    }
}