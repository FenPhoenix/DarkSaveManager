using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace DarkSaveManager;

internal static class Utils
{
    internal static bool EqualsI(this string first, string second) => first.Equals(second, StringComparison.OrdinalIgnoreCase);

    internal static bool StartsWithI(this string first, string second) => first.StartsWith(second, StringComparison.OrdinalIgnoreCase);

    internal static bool EndsWithI(this string first, string second) => first.EndsWith(second, StringComparison.OrdinalIgnoreCase);

    public static Encoding GetOEMCodePageOrFallback(Encoding fallback)
    {
        Encoding enc;
        try
        {
            enc = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage);
        }
        catch
        {
            enc = fallback;
        }

        return enc;
    }

    /// <summary>
    /// Shorthand for <paramref name="value"/>.ToString(<see cref="NumberFormatInfo.InvariantInfo"/>)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToStrInv<T>(this T value) where T : IFormattable
    {
        return value.ToString(null, NumberFormatInfo.InvariantInfo);
    }

    /// <summary>
    /// Shorthand for <paramref name="value"/>.ToString(<see cref="NumberFormatInfo.CurrentInfo"/>)
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToStrCur<T>(this T value) where T : IFormattable
    {
        return value.ToString(null, NumberFormatInfo.CurrentInfo);
    }

    /// <summary>
    /// Use this to run a function to initialize a field without having to create a standalone function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="initFunc"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T RunFunc<T>(Func<T> initFunc) => initFunc.Invoke();
}
