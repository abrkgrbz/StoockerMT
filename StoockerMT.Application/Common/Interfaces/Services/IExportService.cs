using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface IExportService
    {
        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName = "Sheet1");
        Task<byte[]> ExportToCsvAsync<T>(IEnumerable<T> data);
        Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title);
    }
}
