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

namespace api.Services
{
    public interface IAuthServices
    {
        AuthResponeDto login(LoginRequestDto loginRequestDto);
    }

    public class AuthServices : IAuthServices
    {
        private readonly NormalDataBaseContext normalDataBaseContext;

        private readonly JWTServices jWTServices;
        public AuthServices(NormalDataBaseContext normalDataBaseContext, JWTServices jWTServices)
        {
            this.normalDataBaseContext = normalDataBaseContext;
            this.jWTServices = jWTServices;
        }

        public AuthResponeDto login(LoginRequestDto loginRequestDto)
        {

            Users? user = normalDataBaseContext.users.FirstOrDefault(x => x.userName == loginRequestDto.userName && x.password == loginRequestDto.password);
            
            if(user == null)
            {
                return null;
            }
            
            AuthResponeDto response = new AuthResponeDto
            {
                Token = jWTServices.CreateToken(user),
                userName = user.userName,
                email = user.email,
                firstName = user.firstName,
                lastName = user.lastName,
                phoneNumber = user.phoneNumber
            };

            return response;
        }
    }
}