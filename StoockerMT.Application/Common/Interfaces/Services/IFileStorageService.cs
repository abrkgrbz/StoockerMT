using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string fileKey, CancellationToken cancellationToken = default);
        Task DeleteAsync(string fileKey, CancellationToken cancellationToken = default);
    }
}
