using System;
using System.Globalization;
using Foundation;
using Customers.iOS;
using Xamarin.Forms;

[assembly:Dependency(typeof(Localization))]

namespace Customers.iOS
{
    public class Localization : ILocalization
    {
        public CultureInfo GetCurrentCultureInfo()
        {
            var netLanguage = "en";
            if (NSLocale.PreferredLanguages.Length > 0)
            {
                var pref = NSLocale.PreferredLanguages[0];
                netLanguage = pref.Replace("_", "-"); // turns es_ES into es-ES

                if (pref == "pt")
                    pref = "pt-BR"; // get the correct Brazilian language strings from the PCL RESX
                //(note the local iOS folder is still "pt")
            }
            return new CultureInfo(netLanguage);
        }

        public string ToTitleCase(string value)
        {
            return GetCurrentCultureInfo().TextInfo.ToTitleCase(value);
        }
    }
}

