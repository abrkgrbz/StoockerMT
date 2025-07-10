using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Interfaces.Services
{
    public interface IEncryptionService
    {
        // Synchronous methods
        string Encrypt(string plainText);
        string Decrypt(string cipherText);

        // Asynchronous methods for Key Vault
        Task<string> EncryptAsync(string plainText);
        Task<string> DecryptAsync(string cipherText);

        // Password hashing (one-way)
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);

        // Generate secure tokens
        string GenerateSecureToken(int length = 32);
        string GenerateSecurePassword(int length = 16);
    }
}
