using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Helpers
{
    public static class StringHelper
    {
        public static string GenerateRandomString(int length, bool includeNumbers = true, bool includeSpecialChars = false)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string numbers = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var chars = letters;
            if (includeNumbers) chars += numbers;
            if (includeSpecialChars) chars += specialChars;

            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string GenerateSlug(string phrase)
        {
            string str = phrase.ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
            str = Regex.Replace(str, @"\s", "-");
            return str;
        }

        public static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
                return string.Empty;

            if (words.Length == 1)
                return words[0][0].ToString().ToUpper();

            return string.Join("", words.Take(2).Select(w => w[0])).ToUpper();
        }

        public static string HashString(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(bytes);
        }
    }
}
