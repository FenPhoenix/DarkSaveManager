namespace DarkSaveManager;

internal static class Paths
{
    internal static readonly string Startup = AppContext.BaseDirectory;

    internal static readonly string Data = Path.Combine(Startup, "Data");

    internal static readonly string SaveStore = Path.Combine(Data, "SaveStore");

    internal static readonly string Temp = Path.Combine(Startup, "Temp");

    internal static readonly string ConfigIni = Path.Combine(Data, "Config.ini");

    internal static readonly string LogFile = Path.Combine(Data, "Log.txt");
}
