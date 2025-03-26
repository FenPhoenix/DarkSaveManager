using System.Text;
using System.Text.RegularExpressions;

namespace DarkSaveManager;

internal static partial class Core
{
    internal static MainForm View = null!;

    // TODO: Implement list of them, one for each game path
    internal static readonly FileSystemWatcher Thief2Watcher = new();

    internal static void Init()
    {
        Thief2Watcher.Path = Config.Thief2Path;
        Thief2Watcher.Filter = "*.sav";
        Thief2Watcher.IncludeSubdirectories = true;
        Thief2Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
        Thief2Watcher.Changed += Thief2Watcher_Changed;
        Thief2Watcher.Created += Thief2Watcher_Created;
        Thief2Watcher.Deleted += Thief2Watcher_Deleted;
        Thief2Watcher.Renamed += Thief2Watcher_Renamed;

        View = new MainForm();
        View.Show();

        View.RefreshInGameSavesList(GetSaveData(Config.Thief2Path));

        Thief2Watcher.EnableRaisingEvents = true;
    }

    private static void Thief2Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveData(Config.Thief2Path));
    }

    private static void Thief2Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveData(Config.Thief2Path));
    }

    private static void Thief2Watcher_Created(object sender, FileSystemEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveData(Config.Thief2Path));
    }

    private static void Thief2Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveData(Config.Thief2Path));
    }

    internal static List<SaveData> GetSaveData(string gamePath)
    {
        string savePath = Path.Combine(gamePath, "saves");
        string[] saveFiles = Directory.GetFiles(savePath, "*.sav");
        if (AnyInvalidlyNamedSaveFiles(saveFiles))
        {
            // TODO: We should just ignore invalidly named saves instead of disallowing them, this is just for
            //  debug
            throw new InvalidDataException("*** At least one invalidly named save file found");
        }

        List<SaveData> ret = new(saveFiles.Length);
        foreach (string saveFile in saveFiles)
        {
            (bool success, string saveName) = GetSaveName(saveFile);
            if (success)
            {
                SaveData saveData = new(saveFile, Path.GetFileName(saveFile), saveName);
                ret.Add(saveData);
            }
        }

        return ret;
    }

    private static bool AnyInvalidlyNamedSaveFiles(string[] saveFiles)
    {
        foreach (string saveFile in saveFiles)
        {
            string fileName = Path.GetFileName(saveFile);
            if (!NumberedSaveGameNameRegex().Match(fileName).Success &&
                !fileName.Equals("quick.sav"))
            {
                return true;
            }
        }

        return false;
    }

    private static (bool Success, string SaveName) GetSaveName(string saveFile)
    {
        try
        {
            Span<byte> chunkHeaderBuffer = stackalloc byte[12];

            using FileStream stream = File.OpenRead(saveFile);
            using BinaryReader reader = new(stream);
            uint tocOffset = reader.ReadUInt32();
            stream.Position = tocOffset;
            uint invCount = reader.ReadUInt32();
            for (int i = 0; i < invCount; i++)
            {
                stream.ReadExactly(chunkHeaderBuffer);
                uint offset = reader.ReadUInt32();
                // TODO: Check validity: length fits in int32
                uint length = reader.ReadUInt32();

                if (!chunkHeaderBuffer.SequenceEqual("SAVEDESC\0\0\0\0"u8))
                {
                    continue;
                }

                // TODO: Check validity: names match, rest of header format matches
                stream.Position = offset + 24;

                // TODO: Cache this
                byte[] nameBuffer = new byte[1024];

                stream.ReadExactly(nameBuffer);

                int nameEndIndex = Array.IndexOf(nameBuffer, 0);
                int nameLength = nameEndIndex == -1 ? nameBuffer.Length : nameEndIndex + 1;

                // The T2 source code doesn't seem to explicitly do any encoding stuff at all - save names seem
                // to be read in the OEM codepage (850 in my case): 8B == ï
                return (true, Utils.GetOEMCodePageOrFallback(Encoding.UTF8).GetString(nameBuffer, 0, nameLength));
            }

            return (false, "");
        }
        catch
        {
            return (false, "");
        }
    }

    [GeneratedRegex(@"^game[0-9]{4}\.sav$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex NumberedSaveGameNameRegex();

    internal static void Shutdown()
    {
        Application.Exit();
    }
}
