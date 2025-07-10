using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BCrypt.Net;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StoockerMT.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Services
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IDataProtector _dataProtector;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EncryptionService> _logger;
        private readonly SecretClient _keyVaultClient;
        private readonly bool _useKeyVault;

        public EncryptionService(
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration,
            ILogger<EncryptionService> logger)
        {
            _dataProtector = dataProtectionProvider.CreateProtector("StoockerMT.Encryption");
            _configuration = configuration;
            _logger = logger;
             
            var keyVaultUri = _configuration["KeyVault:Uri"];
            _useKeyVault = !string.IsNullOrEmpty(keyVaultUri);

            if (_useKeyVault)
            {
                try
                {
                    _keyVaultClient = new SecretClient(
                        new Uri(keyVaultUri),
                        new DefaultAzureCredential()
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to initialize Key Vault client. Falling back to local encryption.");
                    _useKeyVault = false;
                }
            }
        }
         
        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            try
            {
                return _dataProtector.Protect(plainText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encryption failed");
                throw new InvalidOperationException("Encryption failed", ex);
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            try
            {
                return _dataProtector.Unprotect(cipherText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed");
                throw new InvalidOperationException("Decryption failed", ex);
            }
        }
         
        public async Task<string> EncryptAsync(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            if (_useKeyVault)
            {
                try
                { 
                    var secretName = $"encrypted-{Guid.NewGuid():N}";
                    var secret = new KeyVaultSecret(secretName, plainText);

                    await _keyVaultClient.SetSecretAsync(secret); 
                    return $"vault://{secretName}";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Key Vault encryption failed, falling back to local encryption");
                }
            }
             
            return Encrypt(plainText);
        }

        public async Task<string> DecryptAsync(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;
             
            if (_useKeyVault && cipherText.StartsWith("vault://"))
            {
                try
                {
                    var secretName = cipherText.Substring(8); // Remove "vault://" prefix
                    var secret = await _keyVaultClient.GetSecretAsync(secretName);
                    return secret.Value.Value;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Key Vault decryption failed");
                    throw new InvalidOperationException("Decryption failed", ex);
                }
            } 
            return Decrypt(cipherText);
        } 
        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
             
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password verification failed");
                return false;
            }
        }
         
        public string GenerateSecureToken(int length = 32)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public string GenerateSecurePassword(int length = 16)
        {
            const string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$%^&*";
            var password = new StringBuilder(length);

            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);

            foreach (var b in bytes)
            {
                password.Append(validChars[b % validChars.Length]);
            }

            return password.ToString();
        }
    }
}
