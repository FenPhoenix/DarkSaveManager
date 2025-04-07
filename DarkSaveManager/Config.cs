using System.Reflection;

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
    private const BindingFlags _bFlagsEnum =
        BindingFlags.Instance |
        BindingFlags.Static |
        BindingFlags.Public |
        BindingFlags.NonPublic;

    private static void SetEnumValue<T>(string value, Type type, ref T configField)
    {
        if (type.GetField(value, _bFlagsEnum)?.GetValue(null) is T finalValue)
        {
            configField = finalValue;
        }
    }

    internal static void ReadIni()
    {
        try
        {
            using StreamReader sr = new(Paths.ConfigIni);
            while (sr.ReadLine() is { } line)
            {
                string lineT = line.Trim();
                if (lineT.TryGetValueO("GamePath=", out string value))
                {
                    Config.GamePath = value;
                }
                else if (lineT.TryGetValueO("VisualTheme=", out value))
                {
                    SetEnumValue(value, typeof(VisualTheme), ref Config.VisualTheme);
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
        sw.WriteLine("VisualTheme=" + Config.VisualTheme);
    }
}
