using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Extensions
{
    public static class StringExtensions
    {
        public static string ToKebabCase(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return Regex.Replace(
                    value,
                    "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                    "-$1",
                    RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }

        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value) || !char.IsUpper(value[0]))
                return value;

            char[] chars = value.ToCharArray();
            chars[0] = char.ToLower(chars[0]);
            return new string(chars);
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
    }
}
