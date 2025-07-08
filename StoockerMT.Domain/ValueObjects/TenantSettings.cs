using System;
using System.Collections.Generic;
using System.Linq;

namespace StoockerMT.Domain.ValueObjects
{
    /// <summary>
    /// EF Core compatible TenantSettings value object
    /// </summary>
    public class TenantSettings : BaseValueObject
    {
        // General Settings
        public string TimeZone { get; private set; }
        public string DateFormat { get; private set; }
        public string TimeFormat { get; private set; }
        public string Language { get; private set; }
        public string Currency { get; private set; }

        // Business Settings
        public int FiscalYearStartMonth { get; private set; }
        public DayOfWeek FirstDayOfWeek { get; private set; }
        public string DefaultCountry { get; private set; }
        public string DefaultCity { get; private set; }

        // Display Settings
        public string ThemeColor { get; private set; }
        public string LogoUrl { get; private set; }
        public int ItemsPerPage { get; private set; }
        public bool ShowGridLines { get; private set; }

        // Notification Settings
        public bool EmailNotificationsEnabled { get; private set; }
        public bool SmsNotificationsEnabled { get; private set; }
        public string NotificationEmail { get; private set; }

        // Security Settings
        public int PasswordMinLength { get; private set; }
        public bool RequireTwoFactor { get; private set; }
        public int SessionTimeoutMinutes { get; private set; }
        public int MaxLoginAttempts { get; private set; }

        // Module-Specific Settings (stored as JSON string for EF Core compatibility)
        public string ModuleSettingsJson { get; private set; }

        // Private field for module settings dictionary
        private Dictionary<string, object> _moduleSettings;
        public Dictionary<string, object> ModuleSettings => _moduleSettings ??= DeserializeModuleSettings();

        // Private constructor for EF Core
        private TenantSettings()
        {
            // EF Core requires a parameterless constructor
            // Set default values
            TimeZone = "UTC";
            DateFormat = "yyyy-MM-dd";
            TimeFormat = "HH:mm";
            Language = "en-US";
            Currency = "USD";
            FiscalYearStartMonth = 1;
            FirstDayOfWeek = DayOfWeek.Monday;
            DefaultCountry = string.Empty;
            DefaultCity = string.Empty;
            ThemeColor = "#2563eb";
            LogoUrl = string.Empty;
            ItemsPerPage = 20;
            ShowGridLines = true;
            EmailNotificationsEnabled = true;
            SmsNotificationsEnabled = false;
            NotificationEmail = string.Empty;
            PasswordMinLength = 8;
            RequireTwoFactor = false;
            SessionTimeoutMinutes = 30;
            MaxLoginAttempts = 5;
            ModuleSettingsJson = "{}";
            _moduleSettings = new Dictionary<string, object>();
        }

        // Private constructor for EF Core that matches property names
        private TenantSettings(
            string timeZone,
            string dateFormat,
            string timeFormat,
            string language,
            string currency,
            int fiscalYearStartMonth,
            DayOfWeek firstDayOfWeek,
            string defaultCountry,
            string defaultCity,
            string themeColor,
            string logoUrl,
            int itemsPerPage,
            bool showGridLines,
            bool emailNotificationsEnabled,
            bool smsNotificationsEnabled,
            string notificationEmail,
            int passwordMinLength,
            bool requireTwoFactor,
            int sessionTimeoutMinutes,
            int maxLoginAttempts,
            string moduleSettingsJson)
        {
            TimeZone = timeZone;
            DateFormat = dateFormat;
            TimeFormat = timeFormat;
            Language = language;
            Currency = currency;
            FiscalYearStartMonth = fiscalYearStartMonth;
            FirstDayOfWeek = firstDayOfWeek;
            DefaultCountry = defaultCountry;
            DefaultCity = defaultCity;
            ThemeColor = themeColor;
            LogoUrl = logoUrl;
            ItemsPerPage = itemsPerPage;
            ShowGridLines = showGridLines;
            EmailNotificationsEnabled = emailNotificationsEnabled;
            SmsNotificationsEnabled = smsNotificationsEnabled;
            NotificationEmail = notificationEmail;
            PasswordMinLength = passwordMinLength;
            RequireTwoFactor = requireTwoFactor;
            SessionTimeoutMinutes = sessionTimeoutMinutes;
            MaxLoginAttempts = maxLoginAttempts;
            ModuleSettingsJson = moduleSettingsJson ?? "{}";
        }

        // Factory method for creating new settings
        public static TenantSettings Create(
            string timeZone,
            string dateFormat,
            string language,
            string currency,
            string timeFormat = "HH:mm",
            int fiscalYearStartMonth = 1,
            DayOfWeek firstDayOfWeek = DayOfWeek.Monday,
            string defaultCountry = null,
            string defaultCity = null,
            string themeColor = "#2563eb",
            string logoUrl = null,
            int itemsPerPage = 20,
            bool showGridLines = true,
            bool emailNotificationsEnabled = true,
            bool smsNotificationsEnabled = false,
            string notificationEmail = null,
            int passwordMinLength = 8,
            bool requireTwoFactor = false,
            int sessionTimeoutMinutes = 30,
            int maxLoginAttempts = 5,
            Dictionary<string, object> moduleSettings = null)
        {
            // Validations
            if (string.IsNullOrWhiteSpace(timeZone))
                throw new ArgumentException("TimeZone cannot be empty", nameof(timeZone));

            if (string.IsNullOrWhiteSpace(dateFormat))
                throw new ArgumentException("DateFormat cannot be empty", nameof(dateFormat));

            if (string.IsNullOrWhiteSpace(language))
                throw new ArgumentException("Language cannot be empty", nameof(language));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency cannot be empty", nameof(currency));

            if (fiscalYearStartMonth < 1 || fiscalYearStartMonth > 12)
                throw new ArgumentOutOfRangeException(nameof(fiscalYearStartMonth), "Fiscal year start month must be between 1 and 12");

            if (itemsPerPage < 5 || itemsPerPage > 100)
                throw new ArgumentOutOfRangeException(nameof(itemsPerPage), "Items per page must be between 5 and 100");

            if (passwordMinLength < 6 || passwordMinLength > 32)
                throw new ArgumentOutOfRangeException(nameof(passwordMinLength), "Password minimum length must be between 6 and 32");

            if (sessionTimeoutMinutes < 5 || sessionTimeoutMinutes > 480)
                throw new ArgumentOutOfRangeException(nameof(sessionTimeoutMinutes), "Session timeout must be between 5 and 480 minutes");

            if (maxLoginAttempts < 3 || maxLoginAttempts > 10)
                throw new ArgumentOutOfRangeException(nameof(maxLoginAttempts), "Max login attempts must be between 3 and 10");

            // Validate timezone
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }
            catch
            {
                throw new ArgumentException($"Invalid timezone: {timeZone}", nameof(timeZone));
            }

            // Validate language code (ISO 639-1)
            if (!IsValidLanguageCode(language))
                throw new ArgumentException($"Invalid language code: {language}", nameof(language));

            // Validate currency code (ISO 4217)
            if (!IsValidCurrencyCode(currency))
                throw new ArgumentException($"Invalid currency code: {currency}", nameof(currency));

            // Validate date format
            if (!IsValidDateFormat(dateFormat))
                throw new ArgumentException($"Invalid date format: {dateFormat}", nameof(dateFormat));

            // Validate theme color (hex color)
            if (!string.IsNullOrEmpty(themeColor) && !IsValidHexColor(themeColor))
                throw new ArgumentException($"Invalid theme color: {themeColor}", nameof(themeColor));

            // Validate email
            if (!string.IsNullOrEmpty(notificationEmail) && !IsValidEmail(notificationEmail))
                throw new ArgumentException($"Invalid notification email: {notificationEmail}", nameof(notificationEmail));

            // Create instance
            var settings = new TenantSettings
            {
                TimeZone = timeZone,
                DateFormat = dateFormat,
                TimeFormat = timeFormat,
                Language = language,
                Currency = currency,
                FiscalYearStartMonth = fiscalYearStartMonth,
                FirstDayOfWeek = firstDayOfWeek,
                DefaultCountry = defaultCountry ?? string.Empty,
                DefaultCity = defaultCity ?? string.Empty,
                ThemeColor = themeColor,
                LogoUrl = logoUrl ?? string.Empty,
                ItemsPerPage = itemsPerPage,
                ShowGridLines = showGridLines,
                EmailNotificationsEnabled = emailNotificationsEnabled,
                SmsNotificationsEnabled = smsNotificationsEnabled,
                NotificationEmail = notificationEmail ?? string.Empty,
                PasswordMinLength = passwordMinLength,
                RequireTwoFactor = requireTwoFactor,
                SessionTimeoutMinutes = sessionTimeoutMinutes,
                MaxLoginAttempts = maxLoginAttempts,
                _moduleSettings = moduleSettings ?? new Dictionary<string, object>(),
                ModuleSettingsJson = SerializeModuleSettings(moduleSettings ?? new Dictionary<string, object>())
            };

            return settings;
        }

        // Factory method for default settings
        public static TenantSettings CreateDefault()
        {
            return Create(
                timeZone: "UTC",
                dateFormat: "yyyy-MM-dd",
                language: "en-US",
                currency: "USD"
            );
        }

        // Methods to create modified copies (immutability)
        public TenantSettings WithTimeZone(string timeZone)
        {
            return Create(
                timeZone,
                DateFormat,
                Language,
                Currency,
                TimeFormat,
                FiscalYearStartMonth,
                FirstDayOfWeek,
                DefaultCountry,
                DefaultCity,
                ThemeColor,
                LogoUrl,
                ItemsPerPage,
                ShowGridLines,
                EmailNotificationsEnabled,
                SmsNotificationsEnabled,
                NotificationEmail,
                PasswordMinLength,
                RequireTwoFactor,
                SessionTimeoutMinutes,
                MaxLoginAttempts,
                ModuleSettings
            );
        }

        public TenantSettings WithDisplaySettings(string themeColor = null, string logoUrl = null, int? itemsPerPage = null)
        {
            return Create(
                TimeZone,
                DateFormat,
                Language,
                Currency,
                TimeFormat,
                FiscalYearStartMonth,
                FirstDayOfWeek,
                DefaultCountry,
                DefaultCity,
                themeColor ?? ThemeColor,
                logoUrl ?? LogoUrl,
                itemsPerPage ?? ItemsPerPage,
                ShowGridLines,
                EmailNotificationsEnabled,
                SmsNotificationsEnabled,
                NotificationEmail,
                PasswordMinLength,
                RequireTwoFactor,
                SessionTimeoutMinutes,
                MaxLoginAttempts,
                ModuleSettings
            );
        }

        public TenantSettings WithSecuritySettings(
            int? passwordMinLength = null,
            bool? requireTwoFactor = null,
            int? sessionTimeoutMinutes = null,
            int? maxLoginAttempts = null)
        {
            return Create(
                TimeZone,
                DateFormat,
                Language,
                Currency,
                TimeFormat,
                FiscalYearStartMonth,
                FirstDayOfWeek,
                DefaultCountry,
                DefaultCity,
                ThemeColor,
                LogoUrl,
                ItemsPerPage,
                ShowGridLines,
                EmailNotificationsEnabled,
                SmsNotificationsEnabled,
                NotificationEmail,
                passwordMinLength ?? PasswordMinLength,
                requireTwoFactor ?? RequireTwoFactor,
                sessionTimeoutMinutes ?? SessionTimeoutMinutes,
                maxLoginAttempts ?? MaxLoginAttempts,
                ModuleSettings
            );
        }

        public TenantSettings WithModuleSetting(string moduleCode, string key, object value)
        {
            var newModuleSettings = new Dictionary<string, object>(ModuleSettings);
            var moduleKey = $"{moduleCode}.{key}";
            newModuleSettings[moduleKey] = value;

            return Create(
                TimeZone,
                DateFormat,
                Language,
                Currency,
                TimeFormat,
                FiscalYearStartMonth,
                FirstDayOfWeek,
                DefaultCountry,
                DefaultCity,
                ThemeColor,
                LogoUrl,
                ItemsPerPage,
                ShowGridLines,
                EmailNotificationsEnabled,
                SmsNotificationsEnabled,
                NotificationEmail,
                PasswordMinLength,
                RequireTwoFactor,
                SessionTimeoutMinutes,
                MaxLoginAttempts,
                newModuleSettings
            );
        }

        public object GetModuleSetting(string moduleCode, string key, object defaultValue = null)
        {
            var moduleKey = $"{moduleCode}.{key}";
            return ModuleSettings.TryGetValue(moduleKey, out var value) ? value : defaultValue;
        }

        // Helper methods
        public DateTime ConvertToTenantTime(DateTime utcTime)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
        }

        public DateTime ConvertToUtc(DateTime tenantTime)
        {
            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(TimeZone);
            return TimeZoneInfo.ConvertTimeToUtc(tenantTime, timeZoneInfo);
        }

        public string FormatDate(DateTime date)
        {
            return date.ToString(DateFormat);
        }

        public string FormatDateTime(DateTime dateTime)
        {
            return dateTime.ToString($"{DateFormat} {TimeFormat}");
        }

        // Serialization helpers
        private static string SerializeModuleSettings(Dictionary<string, object> settings)
        {
            if (settings == null || settings.Count == 0)
                return "{}";

            return System.Text.Json.JsonSerializer.Serialize(settings);
        }

        private Dictionary<string, object> DeserializeModuleSettings()
        {
            if (string.IsNullOrWhiteSpace(ModuleSettingsJson) || ModuleSettingsJson == "{}")
                return new Dictionary<string, object>();

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(ModuleSettingsJson)
                    ?? new Dictionary<string, object>();
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        // Validation methods
        private static bool IsValidLanguageCode(string code)
        {
            return !string.IsNullOrEmpty(code) &&
                   (code.Length == 2 || (code.Length == 5 && code[2] == '-'));
        }

        private static bool IsValidCurrencyCode(string code)
        {
            return !string.IsNullOrEmpty(code) &&
                   code.Length == 3 &&
                   code.All(char.IsUpper);
        }

        private static bool IsValidDateFormat(string format)
        {
            try
            {
                var testDate = DateTime.Now;
                var formatted = testDate.ToString(format);
                return !string.IsNullOrEmpty(formatted);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidHexColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return false;

            if (color.StartsWith("#"))
                color = color.Substring(1);

            return (color.Length == 3 || color.Length == 6) &&
                   color.All(c => "0123456789ABCDEFabcdef".Contains(c));
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        // Value Object equality members
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TimeZone;
            yield return DateFormat;
            yield return TimeFormat;
            yield return Language;
            yield return Currency;
            yield return FiscalYearStartMonth;
            yield return FirstDayOfWeek;
            yield return DefaultCountry ?? string.Empty;
            yield return DefaultCity ?? string.Empty;
            yield return ThemeColor;
            yield return LogoUrl ?? string.Empty;
            yield return ItemsPerPage;
            yield return ShowGridLines;
            yield return EmailNotificationsEnabled;
            yield return SmsNotificationsEnabled;
            yield return NotificationEmail ?? string.Empty;
            yield return PasswordMinLength;
            yield return RequireTwoFactor;
            yield return SessionTimeoutMinutes;
            yield return MaxLoginAttempts;
            yield return ModuleSettingsJson;
        }
    }
}