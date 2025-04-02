namespace DarkSaveManager;

internal static class Global
{
    internal static readonly ConfigData Config = new();

    // TODO: Is this different for SS2?
    internal const ushort QuickSaveIndex = 15;
    internal const ushort SaveSlotCount = 16;

    internal const ushort MaxFriendlySaveNameLength = 1024;

    internal const string QuickSaveFileName = "quick.sav";
}
