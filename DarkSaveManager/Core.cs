using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace DarkSaveManager;

internal static partial class Core
{
    internal static MainForm View = null!;

    internal static readonly List<SaveData> InGameSaveDataList = new();
    internal static readonly List<SaveData> StoredSaveDataList = new();

    // TODO: Implement list of them, one for each game path
    internal static readonly FileSystemWatcher Thief2Watcher = new();
    internal static readonly FileSystemWatcher SaveStoreWatcher = new();

    internal static void Init()
    {
        Directory.CreateDirectory(Paths.SaveStore);
        Directory.CreateDirectory(Paths.Temp);

        Thief2Watcher.Path = Config.Thief2Path;
        Thief2Watcher.Filter = "*.sav";
        Thief2Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime;
        Thief2Watcher.Changed += Thief2Watcher_Changed;
        Thief2Watcher.Created += Thief2Watcher_Changed;
        Thief2Watcher.Deleted += Thief2Watcher_Changed;
        Thief2Watcher.Renamed += Thief2Watcher_Changed;

        SaveStoreWatcher.Path = Paths.SaveStore;
        SaveStoreWatcher.Filter = "*.sav_*";
        SaveStoreWatcher.NotifyFilter =
            NotifyFilters.LastWrite
            | NotifyFilters.CreationTime
            | NotifyFilters.FileName;
        SaveStoreWatcher.Changed += SaveStoreWatcher_Changed;
        SaveStoreWatcher.Created += SaveStoreWatcher_Changed;
        SaveStoreWatcher.Deleted += SaveStoreWatcher_Changed;
        SaveStoreWatcher.Renamed += SaveStoreWatcher_Changed;

        View = new MainForm();
        View.Show();

        RefreshViewAllLists();

        Thief2Watcher.EnableRaisingEvents = true;
        SaveStoreWatcher.EnableRaisingEvents = true;
    }

    private static void Thief2Watcher_Changed(object sender, FileSystemEventArgs e)
    {
        View.Invoke(RefreshViewInGameList);
    }

    private static void SaveStoreWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        View.Invoke(RefreshViewStoredList);
    }

    internal static void FillSaveDataList(string savePath, List<SaveData> saveDataList, bool stored)
    {
        saveDataList.Clear();

        string pattern = stored ? "*.sav_*" : "*.sav";

        string[] saveFiles = Directory.GetFiles(savePath, pattern);
        if (AnyInvalidlyNamedSaveFiles(saveFiles))
        {
            // TODO: We should just ignore invalidly named saves instead of disallowing them, this is just for
            //  debug
            throw new InvalidDataException("*** At least one invalidly named save file found");
        }

        foreach (string saveFile in saveFiles)
        {
            if (TryGetSaveData(saveFile, out SaveData? saveData))
            {
                saveDataList.Add(saveData);
            }
        }
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

                if (!TryGetGameSaveIndex(fileNameOnly, out ushort index))
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

    private static bool TryGetGameSaveIndex(string fileNameOnly, out ushort index)
    {
        if (fileNameOnly.EqualsI("quick.sav"))
        {
            index = HighestSaveGameIndex;
            return true;
        }
        else if (ushort.TryParse(fileNameOnly.AsSpan(4, 4), NumberStyles.None, NumberFormatInfo.InvariantInfo, out index))
        {
            return true;
        }

        return false;
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

    private static string GetFinalStoredSaveName(SaveData saveData)
    {
        string originalDest = Path.Combine(Paths.SaveStore, saveData.FileName) + "_1";
        string finalDest = originalDest;
        int i = 1;
        while (File.Exists(finalDest) && i < int.MaxValue)
        {
            finalDest = string.Concat(originalDest.AsSpan(0, originalDest.LastIndexOf('_')), "_", i.ToStrInv());
            i++;
        }

        return finalDest;
    }

    // TODO: Handle errors and if it already exists
    internal static void CopySelectedToStore()
    {
        if (View.TryGetSelectedInGameSaveIndex(out int index))
        {
            using (new DisableWatchers())
            {
                // TODO: Validate
                SaveData saveData = InGameSaveDataList[index];
                string finalDest = GetFinalStoredSaveName(saveData);
                File.Copy(saveData.FullPath, finalDest);

                RefreshViewAllLists();
            }
        }
    }

    // TODO: Handle errors and if it already exists
    internal static void MoveSelectedToStore()
    {
        if (View.TryGetSelectedInGameSaveIndex(out int index))
        {
            using (new DisableWatchers())
            {
                // TODO: Validate
                SaveData saveData = InGameSaveDataList[index];
                string finalDest = GetFinalStoredSaveName(saveData);
                File.Move(saveData.FullPath, finalDest);

                RefreshViewAllLists();
            }
        }
    }

    // TODO: Handle errors and if it already exists
    internal static void MoveToStore(SaveData saveData)
    {
        using (new DisableWatchers())
        {
            // TODO: Validate
            string finalDest = GetFinalStoredSaveName(saveData);
            File.Move(saveData.FullPath, finalDest);

            RefreshViewAllLists();
        }
    }

    // TODO: Allow swapping to any save slot - we'll rename the incoming file to match its dest slot number
    // TODO: Allow dragging and dropping too. That's kind of needed for decent any-slot-swap UX.

    internal static void SwapSaveToGame()
    {
        if (View.TryGetSelectedStoredSaveIndex(out int index))
        {
            using (new DisableWatchers())
            {
                // TODO: Validate
                SaveData storedSaveData = StoredSaveDataList[index];
                string tempDest = Path.Combine(Paths.Temp, storedSaveData.FileName.Substring(0, storedSaveData.FileName.LastIndexOf('_')));

                // TODO: Overwrite or notify or?
                File.Move(storedSaveData.FullPath, tempDest, overwrite: true);

                SaveData? gameSaveData = InGameSaveDataList.Find(x => x.FileName.EqualsI(Path.GetFileName(tempDest)));
                if (gameSaveData != null)
                {
                    MoveToStore(gameSaveData);
                }

                string gameDest = Path.Combine(Config.Thief2Path, Path.GetFileName(tempDest));
                File.Move(tempDest, gameDest);

                RefreshViewAllLists();
            }
        }
    }

    private static void RefreshViewAllLists()
    {
        RefreshViewInGameList();
        RefreshViewStoredList();
    }

    private static void RefreshViewInGameList()
    {
        FillSaveDataList(Config.Thief2Path, InGameSaveDataList, stored: false);
        View.RefreshInGameSavesList(InGameSaveDataList);
    }

    private static void RefreshViewStoredList()
    {
        FillSaveDataList(Paths.SaveStore, StoredSaveDataList, stored: true);
        View.RefreshSaveStoreList(StoredSaveDataList);
    }

    [GeneratedRegex(@"^game[0-9]{4}\.sav(_[0-9]+)?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
    private static partial Regex NumberedSaveGameNameRegex();

    internal static void Shutdown()
    {
        Application.Exit();
    }

    private sealed class DisableWatchers : IDisposable
    {
        private static readonly Lock _lock = new();
        private static int _count;

        private readonly bool oldGameEventWatchingValue;
        private readonly bool oldStoreEventWatchingValue;

        public DisableWatchers()
        {
            lock (_lock)
            {
                if (_count == 0)
                {
                    oldGameEventWatchingValue = Thief2Watcher.EnableRaisingEvents;
                    oldStoreEventWatchingValue = SaveStoreWatcher.EnableRaisingEvents;

                    Thief2Watcher.EnableRaisingEvents = false;
                    SaveStoreWatcher.EnableRaisingEvents = false;
                }
                _count++;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_count == 1)
                {
                    Thief2Watcher.EnableRaisingEvents = oldGameEventWatchingValue;
                    SaveStoreWatcher.EnableRaisingEvents = oldStoreEventWatchingValue;
                }
                _count--;
            }
        }
    }
}
