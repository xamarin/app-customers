using System;
using System.Globalization;
using Xamarin.Forms;
using Customers.Droid;
using Customers;

[assembly:Dependency(typeof(Localization))]

namespace Customers.Droid
{
    public class Localization : ILocalization
    {
        public CultureInfo GetCurrentCultureInfo ()
        {
            var androidLocale = Java.Util.Locale.Default;
            var netLanguage = androidLocale.ToString().Replace ("_", "-"); // turns pt_BR into pt-BR
            return new CultureInfo(netLanguage);
        }

        public string ToTitleCase(string value)
        {
            return GetCurrentCultureInfo().TextInfo.ToTitleCase(value);
        }
    }
}

