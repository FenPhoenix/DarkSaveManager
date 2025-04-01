namespace DarkSaveManager;

internal static class Paths
{
    internal static readonly string SaveStore = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSaveManager");

    // TODO: Make this a random folder name to avoid naming conflicts
    internal static readonly string Temp = Path.Combine(Path.GetTempPath(), "DarkSaveManager");

    internal static readonly string ConfigIni = Path.Combine(AppContext.BaseDirectory, "Config.ini");
}
