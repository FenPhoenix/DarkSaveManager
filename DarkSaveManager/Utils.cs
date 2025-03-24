namespace DarkSaveManager;

internal static class Utils
{
    internal static bool EqualsI(this string first, string second) => first.Equals(second, StringComparison.OrdinalIgnoreCase);

    internal static bool StartsWithI(this string first, string second) => first.StartsWith(second, StringComparison.OrdinalIgnoreCase);

    internal static bool EndsWithI(this string first, string second) => first.EndsWith(second, StringComparison.OrdinalIgnoreCase);
}
