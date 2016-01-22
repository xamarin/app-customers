using System.Globalization;

namespace Customers
{
    public interface ILocalization
    {
        CultureInfo GetCurrentCultureInfo ();

        string ToTitleCase(string value);
    }
}

