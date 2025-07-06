using StoockerMT.Domain.Entities.MasterDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static StoockerMT.Application.Features.Authentication.DTOs.AuthenticationDtos;

namespace StoockerMT.Application.Common.Security
{
    public interface ITokenService
    {
        TokenGenerationResult GenerateToken(TenantUser user, Tenant tenant, List<string> permissions);
        ClaimsPrincipal ValidateToken(string token); 
    }
}
