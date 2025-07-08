using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StoockerMT.Application.Common.Security;
using StoockerMT.Identity.Configuration;
using StoockerMT.Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using StoockerMT.Application.Features.Authentication.DTOs;
using StoockerMT.Application.Common.Interfaces;
using StoockerMT.Persistence.Services;

namespace StoockerMT.Identity.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            // Configuration
            var jwtSettings = new JwtSettings();
            configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
            services.AddSingleton(jwtSettings);

            var identityConfig = new IdentityConfiguration();
            configuration.GetSection(IdentityConfiguration.SectionName).Bind(identityConfig);
            services.AddSingleton(identityConfig);

            // Services
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<ICurrentTenantService, CurrentTenantService>();
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; // Set to true in production
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Authorization policies
            services.AddAuthorization(options =>
            {
                // Default policy requires authenticated user
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                // Add module-based policies
                options.AddPolicy("CustomerModule", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == AuthenticationDtos.CustomClaimTypes.Permissions &&
                                                  c.Value.StartsWith("Customer."))));

                options.AddPolicy("AccountingModule", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == AuthenticationDtos.CustomClaimTypes.Permissions &&
                                                  c.Value.StartsWith("Accounting."))));

                options.AddPolicy("HRModule", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c => c.Type == AuthenticationDtos.CustomClaimTypes.Permissions &&
                                                  c.Value.StartsWith("HR."))));
            });

            return services;
        }
    }
}
