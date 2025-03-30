namespace DarkSaveManager;

internal static class Global
{
    internal static readonly ConfigData Config = new();

    // TODO: Is this different for SS2?
    internal const ushort QuickSaveIndex = 15;
    internal const int MaxFriendlySaveNameLength = 1024;
    internal const int SaveSlotCount = 16;
}
