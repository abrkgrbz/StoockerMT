using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Application.Common.Validators
{
    public static class CommonValidators
    {
        public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Phone number must be in E.164 format");
        }

        public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .WithMessage("Must be a valid URL");
        }

        public static IRuleBuilderOptions<T, decimal> Money<T>(this IRuleBuilder<T, decimal> ruleBuilder)
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0)
                .PrecisionScale(18, 2, true)
                .WithMessage("Invalid money format");
        }

        public static IRuleBuilderOptions<T, string> TurkishIdentityNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .Length(11)
                .Matches(@"^\d{11}$")
                .Must(BeValidTurkishIdentityNumber)
                .WithMessage("Invalid Turkish identity number");
        }

        private static bool BeValidTurkishIdentityNumber(string identityNumber)
        {
            if (string.IsNullOrEmpty(identityNumber) || identityNumber.Length != 11)
                return false;

            if (!long.TryParse(identityNumber, out _))
                return false;

            if (identityNumber[0] == '0')
                return false;

            int[] digits = identityNumber.Select(c => int.Parse(c.ToString())).ToArray();

            int sumOdd = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
            int sumEven = digits[1] + digits[3] + digits[5] + digits[7];

            int digit10 = ((sumOdd * 7) - sumEven) % 10;
            if (digits[9] != digit10)
                return false;

            int sumFirst10 = digits.Take(10).Sum();
            int digit11 = sumFirst10 % 10;

            return digits[10] == digit11;
        }
    }
}
