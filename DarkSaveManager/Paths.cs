using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DarkSaveManager;

internal static class Paths
{
    internal static readonly string Data = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "DarkSaveManager");

    internal static readonly string SaveStore = Path.Combine(Data, "SaveStore");

    // TODO: Make this a random folder name to avoid naming conflicts
    internal static readonly string Temp = Path.Combine(Path.GetTempPath(), "DarkSaveManager");

    internal static readonly string ConfigIni = Path.Combine(Data, "Config.ini");

    internal static readonly string LogFile = Path.Combine(Data, "Log.txt");
}
