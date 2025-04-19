using System.Reflection;
using System.Runtime.CompilerServices;

namespace DarkSaveManager;

internal sealed class ConfigData
{
    // TODO: Add support for unlimited game paths (including multiple copies of one game)

    //internal string GamePath = "";
    internal readonly List<Game> Games = new();

    internal Game? CurrentGame;

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

    private static void SetEnumValue<T>(string value, ref T configField) where T : notnull
    {
        if (typeof(T).GetField(value, _bFlagsEnum)?.GetValue(null) is T finalValue)
        {
            configField = finalValue;
        }
    }

    private enum ConfigState
    {
        None,
        Global,
        Game,
    }

    internal static void ReadIni()
    {
        try
        {
            ConfigState state = ConfigState.None;

            using StreamReader sr = new(Paths.ConfigIni);
            Game? game = null;
            while (sr.ReadLine() is { } line)
            {
                string lineT = line.Trim();

                switch (lineT)
                {
                    case "[Global]":
                    {
                        state = ConfigState.Global;
                        break;
                    }
                    case "[Game]":
                    {
                        state = ConfigState.Game;
                        AddGameIfNotNull(game);
                        game = new Game();
                        break;
                    }
                    default:
                    {
                        FillField(state, lineT, game);
                        break;
                    }
                }
            }
            AddGameIfNotNull(game);
        }
        catch
        {
            // ignore - file doesn't exist
        }

        return;

        static void FillField(ConfigState state, string lineT, Game? game)
        {
            switch (state)
            {
                case ConfigState.Global:
                {
                    if (lineT.TryGetValueO("VisualTheme=", out string value))
                    {
                        SetEnumValue(value, ref Config.VisualTheme);
                    }
                    break;
                }
                case ConfigState.Game:
                {
                    if (game == null) return;

                    if (lineT.TryGetValueO("Name=", out string value))
                    {
                        game.Name = value;
                    }
                    else if (lineT.TryGetValueO("GameSavesPath=", out value))
                    {
                        game.GameSavesPath = value;
                    }
                    else if (lineT.TryGetValueO("StoredSavesDirName=", out value))
                    {
                        game.StoredSavesDirName = value;
                    }
                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void AddGameIfNotNull(Game? game)
        {
            if (game == null) return;
            Config.Games.Add(game);
        }
    }

    internal static void WriteIni()
    {
        using StreamWriter sw = new(Paths.ConfigIni);
        sw.WriteLine("[Global]");
        sw.WriteLine("VisualTheme=" + Config.VisualTheme);

        foreach (Game game in Config.Games)
        {
            sw.WriteLine();
            sw.WriteLine("[Game]");
            sw.WriteLine("Name=" + game.Name);
            sw.WriteLine("GameSavesPath=" + game.GameSavesPath);
            sw.WriteLine("StoredSavesDirName=" + game.StoredSavesDirName);
        }
    }
}
