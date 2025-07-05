using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
        bool IsAuthenticated { get; }
    }
}
