using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Identity.Configuration
{
    public class IdentityConfiguration
    {
        public const string SectionName = "Identity";

        public int PasswordMinLength { get; set; } = 8;
        public bool PasswordRequireDigit { get; set; } = true;
        public bool PasswordRequireUppercase { get; set; } = true;
        public bool PasswordRequireLowercase { get; set; } = true;
        public bool PasswordRequireSpecialChar { get; set; } = true;
        public int MaxFailedLoginAttempts { get; set; } = 5;
        public int LockoutDurationInMinutes { get; set; } = 15;
        public bool EnableRefreshToken { get; set; } = true;
    }
}
