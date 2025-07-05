using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using StoockerMT.Application.Common.Interfaces;

namespace StoockerMT.Infrastructure.Services
{
    public class DateTimeService : IDateTime
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration; 

        public DateTime Now
        {
            get
            {
                var timeZoneId = GetTimeZoneId();
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            }
        }

        public DateTime UtcNow => DateTime.UtcNow;

        private string GetTimeZoneId()
        {
            // 1. Try to get from HTTP header
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var timeZoneHeader = httpContext.Request.Headers["X-TimeZone"].FirstOrDefault();
                if (!string.IsNullOrEmpty(timeZoneHeader))
                {
                    try
                    {
                        TimeZoneInfo.FindSystemTimeZoneById(timeZoneHeader);
                        return timeZoneHeader;
                    }
                    catch
                    {
                        // Invalid timezone, continue to next option
                    }
                }
            }
             
            var userTimeZone = httpContext?.User?.FindFirst("TimeZone")?.Value;
            if (!string.IsNullOrEmpty(userTimeZone))
            {
                return userTimeZone;
            }
             
            var defaultTimeZone = _configuration["Application:DefaultTimeZone"];
            if (!string.IsNullOrEmpty(defaultTimeZone))
            {
                return defaultTimeZone;
            }
             
            return "Turkey Standard Time";
        }
    }
} 
