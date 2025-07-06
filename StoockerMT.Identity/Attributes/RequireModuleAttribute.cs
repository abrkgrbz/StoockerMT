using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StoockerMT.Application.Features.Authentication.DTOs.AuthenticationDtos;

namespace StoockerMT.Identity.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireModuleAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _moduleCode;

        public RequireModuleAttribute(string moduleCode)
        {
            _moduleCode = moduleCode;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var hasModuleAccess = user.Claims
                .Where(c => c.Type == CustomClaimTypes.Permissions)
                .Any(c => c.Value.StartsWith($"{_moduleCode}."));

            if (!hasModuleAccess)
            {
                context.Result = new ObjectResult(new
                {
                    error = "Module access denied",
                    module = _moduleCode,
                    message = "You don't have access to this module"
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
