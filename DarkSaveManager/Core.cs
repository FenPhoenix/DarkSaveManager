using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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

        View.RefreshInGameSavesList(GetSaveDataList(Config.Thief2Path));

        Thief2Watcher.EnableRaisingEvents = true;
    }

    private static void Thief2Watcher_Renamed(object sender, RenamedEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveDataList(Config.Thief2Path));
    }

    private static void Thief2Watcher_Deleted(object sender, FileSystemEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveDataList(Config.Thief2Path));
    }

    private static void Thief2Watcher_Created(object sender, FileSystemEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveDataList(Config.Thief2Path));
    }

    private static void Thief2Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        View.RefreshInGameSavesList(GetSaveDataList(Config.Thief2Path));
    }

    internal static List<SaveData> GetSaveDataList(string gamePath)
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
            if (TryGetSaveData(saveFile, out SaveData? saveData))
            {
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

    private static bool TryGetSaveData(string fullPath, [NotNullWhen(true)] out SaveData? saveData)
    {
        saveData = null;

        try
        {
            using FileStream stream = File.OpenRead(fullPath);

            if (!TrySeekToSaveName(stream)) return false;

            const int maxNameLength = 1024;

            byte[] nameBuffer = ArrayPool<byte>.Shared.Rent(maxNameLength);
            try
            {
                stream.ReadExactly(nameBuffer, 0, maxNameLength);

                int nameEndIndex = Array.IndexOf<byte>(nameBuffer, 0, maxNameLength);
                int nameLength = nameEndIndex == -1 ? maxNameLength : nameEndIndex + 1;

                // The T2 source code doesn't seem to explicitly do any encoding stuff at all - save names seem
                // to be read in the OEM codepage (850 in my case): 8B == ï
                string saveName = Utils.GetOEMCodePageOrFallback(Encoding.UTF8).GetString(nameBuffer, 0, nameLength);

                string fileNameOnly = Path.GetFileName(fullPath);

                if (!ushort.TryParse(fileNameOnly.AsSpan(4, 4), NumberStyles.None, NumberFormatInfo.InvariantInfo, out ushort index))
                {
                    return false;
                }

                saveData = new SaveData(index, fullPath, fileNameOnly, saveName);
                return true;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(nameBuffer);
            }
        }
        catch
        {
            return false;
        }
    }

    private static bool TrySeekToSaveName(Stream stream)
    {
        try
        {
            Span<byte> chunkHeaderBuffer = stackalloc byte[12];

            using BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen: true);

            uint tocOffset = reader.ReadUInt32();

            stream.Position = tocOffset;

            uint invCount = reader.ReadUInt32();

            for (int i = 0; i < invCount; i++)
            {
                stream.ReadExactly(chunkHeaderBuffer);

                uint offset = reader.ReadUInt32();

                uint length = reader.ReadUInt32();
                if (length > int.MaxValue)
                {
                    return false;
                }

                if (!chunkHeaderBuffer.SequenceEqual("SAVEDESC\0\0\0\0"u8))
                {
                    continue;
                }

                stream.Position = offset;
                stream.ReadExactly(chunkHeaderBuffer);

                // TODO: Dedupe
                if (!chunkHeaderBuffer.SequenceEqual("SAVEDESC\0\0\0\0"u8))
                {
                    return false;
                }

                stream.Seek(12, SeekOrigin.Current);
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    [GeneratedRegex(@"^game[0-9]{4}\.sav$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex NumberedSaveGameNameRegex();

    internal static void Shutdown()
    {
        Application.Exit();
    }
}
