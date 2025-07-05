using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.DTOs
{
    public abstract class AuditableDto : BaseDto
    {
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
