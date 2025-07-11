using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantResponseData
    {
        public int TenantId { get; set; }
        public string TenantCode { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
