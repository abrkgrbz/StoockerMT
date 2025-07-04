using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.Entities.Common
{
    public interface IAuditableEntity
    {
        string CreatedBy { get; set; }
        string? UpdatedBy { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
