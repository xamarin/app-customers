using System;

using Xamarin.Forms;
using System.Linq;

namespace Customers
{
    public static class StringExtensions
    {
        public static string SanitizePhoneNumber(this string value)
        {
            return new String(value.ToCharArray().Where(Char.IsDigit).ToArray());
        }

        public static string ToTitleCase(this string input)
        {
            return DependencyService.Get<ILocalization>().ToTitleCase(input);
        }

        public static bool IsNullOrWhiteSpace(this string value) 
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}


