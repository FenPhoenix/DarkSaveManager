namespace DarkSaveManager;

internal static class Global
{
    internal static readonly ConfigData Config = new();

    // TODO: Is this different for SS2?
    internal const ushort HighestSaveGameIndex = 15;
}
