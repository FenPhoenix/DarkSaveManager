using System.Buffers;
using JetBrains.Annotations;

namespace DarkSaveManager;

internal sealed class SaveData
{
    internal readonly ushort SlotIndex;
    internal readonly string FullPath;
    internal readonly string FileName;
    internal readonly string FriendlySaveName;

    internal SaveData(ushort slotIndex, string fullPath, string fileName, string friendlySaveName)
    {
        SlotIndex = slotIndex;
        FullPath = fullPath;
        FileName = fileName;
        FriendlySaveName = friendlySaveName;
    }
}

[PublicAPI]
public readonly ref struct RentScope<T>
{
    public readonly T[] Array;
    public readonly int Length;
    public readonly Span<T> Span;

    public RentScope(int length)
    {
        Array = ArrayPool<T>.Shared.Rent(length);
        Length = length;
        Span = Array.AsSpan(0, Length);
    }

    public RentScope(int length, bool clear)
    {
        Array = ArrayPool<T>.Shared.Rent(length);
        Length = length;
        Span = Array.AsSpan(0, Length);
        if (clear) Span.Clear();
    }

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(Array);
    }
}

[PublicAPI]
public static class ByteSize
{
    public const int KB = 1024;
    public const int MB = KB * 1024;
    public const int GB = MB * 1024;
}

public static class ByteLengths
{
    public const int Byte = 1;
    public const int Int16 = 2;
    public const int Int32 = 4;
    public const int Int64 = 8;
}

public enum SortDirection
{
    Ascending,
    Descending,
}

public enum WindowState
{
    Normal,
    Minimized,
    Maximized,
}

public enum MBoxIcon
{
    None,
    Error,
    Warning,
    Information,
}

public enum MBoxButtons
{
    OK,
    OKCancel,
    YesNoCancel,
    YesNo,
}

public enum MBoxButton
{
    Yes,
    No,
    Cancel,
}

public enum VisualTheme
{
    Classic,
    Dark,
}
