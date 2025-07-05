using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface IReportingService
    {
        Task<byte[]> GeneratePdfReportAsync(string reportName, object parameters, CancellationToken cancellationToken = default);
        Task<byte[]> GenerateExcelReportAsync(string reportName, object parameters, CancellationToken cancellationToken = default);
        Task<string> GenerateHtmlReportAsync(string reportName, object parameters, CancellationToken cancellationToken = default);
    }

}
