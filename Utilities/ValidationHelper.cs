using System;

namespace Utilities
{
    public static class ValidationHelper
    {
        public static bool IsValidDecimal(string value)
        {
            return decimal.TryParse(value, out decimal result) && result >= 0;
        }

        public static bool IsValidDate(string value)
        {
            return DateTime.TryParse(value, out DateTime result) && result <= DateTime.Now;
        }

        public static bool IsValidNumber(string value)
        {
            return long.TryParse(value, out long result) && result > 0;
        }

        public static string SanitizeInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Trim();
        }
    }
}