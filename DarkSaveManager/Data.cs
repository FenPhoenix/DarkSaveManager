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