using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using JetBrains.Annotations;

namespace DarkSaveManager;

internal static partial class Utils
{
    internal static bool EqualsI(this string first, string second) => first.Equals(second, StringComparison.OrdinalIgnoreCase);

    internal static bool EqualsI(this ReadOnlySpan<char> first, ReadOnlySpan<char> second) => first.Equals(second, StringComparison.OrdinalIgnoreCase);

    internal static bool StartsWithI(this string first, string second) => first.StartsWith(second, StringComparison.OrdinalIgnoreCase);

    internal static bool StartsWithI(this ReadOnlySpan<char> first, ReadOnlySpan<char> second) => first.StartsWith(second, StringComparison.OrdinalIgnoreCase);

    internal static bool EndsWithI(this string first, string second) => first.EndsWith(second, StringComparison.OrdinalIgnoreCase);

    internal static bool EndsWithI(this ReadOnlySpan<char> first, ReadOnlySpan<char> second) => first.EndsWith(second, StringComparison.OrdinalIgnoreCase);

    #region Empty / whitespace checks

    /// <summary>
    /// Returns true if <paramref name="value"/> is null or empty.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [ContractAnnotation("null => true")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEmpty([NotNullWhen(false)] this string? value) => string.IsNullOrEmpty(value);

    /// <summary>
    /// Returns true if <paramref name="value"/> is null, empty, or whitespace.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [ContractAnnotation("null => true")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsWhiteSpace([NotNullWhen(false)] this string? value) => string.IsNullOrWhiteSpace(value);

    public static bool IsWhiteSpace(this ReadOnlySpan<byte> span)
    {
        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsWhiteSpace((char)span[i])) return false;
        }
        return true;
    }

    #endregion

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

    /// <summary>
    /// Clamps a number to between <paramref name="min"/> and <paramref name="max"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static T Clamp<T>(this T value, T min, T max) where T : INumber<T> => T.Clamp(value, min, max);

    /// <summary>
    /// If <paramref name="value"/> is less than zero, returns zero. Otherwise, returns <paramref name="value"/>
    /// unchanged.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ClampToZero<T>(this T value) where T : INumber<T> => T.Max(value, T.Zero);

    /*
    We'd like to be able to say "GetAndAddIfNeeded(key, valueToAdd)", but then we'd have to pass an instantiated
    value with every call, defeating the purpose of a dictionary-as-cache. So the best we can do is to use this
    pattern:

    var value = dict.TryGetValue(key, out var result)
        ? result
        : dict.AddAndReturn(key, the_default_value);
    */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TValue AddAndReturn<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value) where TKey : notnull
    {
        dict[key] = value;
        return value;
    }

    #region Dispose and clear

    public static void DisposeAll<T>(this T[] array) where T : IDisposable?
    {
        foreach (T item in array)
        {
            item?.Dispose();
        }
    }

    public static void DisposeRange<T>(this T[] array, int start, int end) where T : IDisposable?
    {
        for (int i = start; i < end; i++)
        {
            array[i]?.Dispose();
        }
    }

    #endregion

    public static int GetPercentFromValue_Int(int current, int total) => total == 0 ? 0 : (100 * current) / total;
    public static float GetValueFromPercent_Float(float percent, int total) => (percent / 100f) * total;

    internal static int MathMax3(int num1, int num2, int num3) => Math.Max(Math.Max(num1, num2), num3);

    internal static int MathMax4(int num1, int num2, int num3, int num4) => Math.Max(Math.Max(Math.Max(num1, num2), num3), num4);
}
