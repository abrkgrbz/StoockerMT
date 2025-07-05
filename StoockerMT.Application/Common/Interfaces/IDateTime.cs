using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces
{
    public interface IDateTime
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
         
        DateTime Today => Now.Date;
        DateTime UtcToday => UtcNow.Date;
         
        DateOnly DateNow => DateOnly.FromDateTime(Now);
        DateOnly DateUtcNow => DateOnly.FromDateTime(UtcNow);

        TimeOnly TimeNow => TimeOnly.FromDateTime(Now);
        TimeOnly TimeUtcNow => TimeOnly.FromDateTime(UtcNow);
    }
}
