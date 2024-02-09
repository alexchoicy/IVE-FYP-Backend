using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using api.Models.Entity.NormalDB;
using Microsoft.IdentityModel.Tokens;

namespace api.utils
{
    public class JWTServices
    {
        private readonly SymmetricSecurityKey key;
        private readonly string issuer;
        private readonly string audience;
        public JWTServices(IConfiguration config)
        {
            string config_key = config["Jwt:Key"] ?? "";
            string config_issuer = config["Jwt:Issuer"] ?? "";
            string config_audience = config["Jwt:Audience"] ?? "";
            if (config_key == "" || config_issuer == "" || config_audience == "")
            {
                throw new Exception("JWT Key, Issuer, Audience is not found in appsettings.json");
            }
            this.issuer = config_issuer;
            this.audience = config_audience;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config_key));
        }

        public string CreateToken(Users users, int expireDays = 30)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, users.userID.ToString()),
                new Claim("type", "access-token")
            };

            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(expireDays),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string CreateTokenForReset(Users user, int expireHours = 20)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.userID.ToString()),
                new Claim("type", "password-reset")
            };

            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(expireHours),
                SigningCredentials = credentials,
                Issuer = issuer,
                Audience = audience
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        
        }
    }
    public static class JWTServicesExtension
    {
        static JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        public static string? getUserID(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
        public static string? getUserIDByToken(string token)
        {
            JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken ?? throw new Exception("Invalid Token");
            return securityToken?.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value;
        }
        
        public static string? getType(this ClaimsPrincipal user)
        {
            return user.Claims.FirstOrDefault(x => x.Type == "type")?.Value;
        }

        public static string? getTypeByToken(string token)
        {
            JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken ?? throw new Exception("Invalid Token");
            return securityToken?.Claims.FirstOrDefault(x => x.Type == "type")?.Value;
        } 

        public static string? getExpireTimeByToken(string token)
        {
            JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken ?? throw new Exception("Invalid Token");
            return securityToken?.Claims.FirstOrDefault(x => x.Type == "exp")?.Value;
        }

        public static string getAllCliams(this ClaimsPrincipal user)
        {
            string result = "";
            foreach (Claim claim in user.Claims)
            {
                result += $"{claim.Type}: {claim.Value}\n";
            }
            return result;
        }
        public static string getAllCliamsByToken(string token)
        {
            JwtSecurityToken securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken ?? throw new Exception("Invalid Token");
            string result = "";
            foreach (Claim claim in securityToken.Claims)
            {
                result += $"{claim.Type}: {claim.Value}\n";
            }
            return result;
        }
    }
}