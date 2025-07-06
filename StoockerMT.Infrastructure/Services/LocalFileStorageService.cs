using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Infrastructure.Services
{
    public class LocalFileStorageService : IFileStorageService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalFileStorageService> _logger;
        private readonly string _basePath;

        public LocalFileStorageService(IConfiguration configuration, ILogger<LocalFileStorageService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _basePath = _configuration["Storage:LocalPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public async Task<string> UploadAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
        {
            try
            {
                var fileKey = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
                var filePath = Path.Combine(_basePath, fileKey);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileStream.CopyToAsync(stream, cancellationToken);
                }

                _logger.LogInformation("File uploaded successfully: {FileKey}", fileKey);
                return fileKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file: {FileName}", fileName);
                throw;
            }
        }

        public async Task<Stream> DownloadAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var filePath = Path.Combine(_basePath, fileKey);

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {fileKey}");
                }

                return new FileStream(filePath, FileMode.Open, FileAccess.Read);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: {FileKey}", fileKey);
                throw;
            }
        }

        public async Task DeleteAsync(string fileKey, CancellationToken cancellationToken = default)
        {
            try
            {
                var filePath = Path.Combine(_basePath, fileKey);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    _logger.LogInformation("File deleted successfully: {FileKey}", fileKey);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FileKey}", fileKey);
                throw;
            }
        }
    }
}
