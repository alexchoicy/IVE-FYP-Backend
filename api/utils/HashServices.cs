using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace api.utils
{
    public class HashServices
    {
        private string pepper;
        public HashServices(IConfiguration configuration)
        {
            pepper = configuration.GetValue<string>("Password:Pepper") ?? throw new ArgumentNullException("Pepper is not defined");
        }

        public string HashPassword(string password, string salt)
        {
            string passwordSaltPepper = password + salt + pepper;
            byte[] bytes = Encoding.UTF8.GetBytes(passwordSaltPepper);
            byte[] hash = SHA256.HashData(bytes);
            return Convert.ToBase64String(hash);
        }

        public string saltGenerator()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(salt);
        }
    }
}