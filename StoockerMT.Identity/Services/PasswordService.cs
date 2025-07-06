using StoockerMT.Application.Common.Security;
using StoockerMT.Identity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Identity.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly IdentityConfiguration _config;

        public PasswordService(IdentityConfiguration config)
        {
            _config = config;
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }

        public bool ValidatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < _config.PasswordMinLength)
                return false;

            if (_config.PasswordRequireDigit && !password.Any(char.IsDigit))
                return false;

            if (_config.PasswordRequireUppercase && !password.Any(char.IsUpper))
                return false;

            if (_config.PasswordRequireLowercase && !password.Any(char.IsLower))
                return false;

            if (_config.PasswordRequireSpecialChar && !password.Any(ch => !char.IsLetterOrDigit(ch)))
                return false;

            return true;
        }
    }
}
