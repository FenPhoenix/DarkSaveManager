namespace DarkSaveManager;

internal sealed class ConfigData
{
    // TODO: Add support for unlimited game paths (including multiple copies of one game)
    internal string GamePath = "";

    internal bool DarkMode => VisualTheme == VisualTheme.Dark;

    internal VisualTheme VisualTheme = VisualTheme.Dark;

    internal bool FollowSystemTheme = false;
}

internal static class ConfigIni
{
    internal static void ReadIni()
    {
        try
        {
            using StreamReader sr = new(Paths.ConfigIni);
            while (sr.ReadLine() is { } line)
            {
                string lineT = line.Trim();
                if (lineT.StartsWithI("GamePath="))
                {
                    Config.GamePath = lineT.Substring("GamePath=".Length);
                }
            }
        }
        catch
        {
            // ignore - file doesn't exist
        }
    }

    internal static void WriteIni()
    {
        using StreamWriter sw = new(Paths.ConfigIni);
        sw.WriteLine("GamePath=" + Config.GamePath);
    }
}
