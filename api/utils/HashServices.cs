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
        private readonly IConfiguration configuration;
        private string pepper;
        public HashServices(IConfiguration configuration)
        {
            this.configuration = configuration;
            pepper = configuration.GetValue<string>("Password:Pepper") ?? throw new ArgumentNullException("Pepper is not defined");
        }

        public string HashPassword(string password, string salt)
        {
            SHA256 sha256 = SHA256.Create();
            string passwordSaltPepper = password + salt + pepper;
            byte[] bytes = Encoding.UTF8.GetBytes(passwordSaltPepper);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public string saltGenerator()
        {
            byte[] salt = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(salt);
        }
    }
}