using System.Globalization;

namespace BookShop.Application.Сommon.Extensions;

public static class StringExtensions
{
    public static string ToNormalizedTitleCase( this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant();

        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(normalized);
    }
}
