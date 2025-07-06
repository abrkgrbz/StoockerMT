using Microsoft.AspNetCore.Http;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Application.Common.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static StoockerMT.Application.Features.Authentication.DTOs.AuthenticationDtos;
using StoockerMT.Identity.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StoockerMT.Identity.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                await AttachUserToContext(context, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContext(HttpContext context, string token)
        {
            try
            { 
                var tokenService = context.RequestServices.GetRequiredService<ITokenService>();  
                 
                var principal = tokenService.ValidateToken(token);
                if (principal != null)
                { 
                    context.User = principal;
                }
            }
            catch (Exception ex)
            {
                // Log the error but don't throw - invalid token means unauthenticated request
                var logger = context.RequestServices.GetService<Microsoft.Extensions.Logging.ILogger<JwtMiddleware>>();
                logger?.LogWarning(ex, "JWT validation failed");
            }
        }
    }
}
