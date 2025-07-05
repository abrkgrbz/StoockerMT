using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Security
{
    public interface ISecurityService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        string GenerateToken();
        string GenerateRefreshToken();
    }
}
