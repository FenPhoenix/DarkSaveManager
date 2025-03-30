using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32.SafeHandles;

namespace DarkSaveManager;

internal static partial class Core
{
    internal static MainForm View = null!;

    internal static readonly SaveData?[] InGameSaveDataList = new SaveData?[SaveSlotCount];
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

    internal static void FillStoredSaveDataList(string savePath, List<SaveData> saveDataList)
    {
        saveDataList.Clear();

        const string pattern = "*.sav_*";

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

    internal static void FillGameSaveDataList(string savePath, SaveData?[] saveDataList)
    {
        Array.Clear(saveDataList);

        const string pattern = "*.sav";

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
                saveDataList[saveData.Index] = saveData;
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

            if (!TrySeekToFriendlySaveName(stream, out _)) return false;

            byte[] nameBuffer = ArrayPool<byte>.Shared.Rent(MaxFriendlySaveNameLength);
            try
            {
                stream.ReadExactly(nameBuffer, 0, MaxFriendlySaveNameLength);

                int nameEndIndex = Array.IndexOf<byte>(nameBuffer, 0, MaxFriendlySaveNameLength);
                int nameLength = nameEndIndex == -1 ? MaxFriendlySaveNameLength : nameEndIndex + 1;

                // The T2 source code doesn't seem to explicitly do any encoding stuff at all - save names seem
                // to be read in the OEM codepage (850 in my case): 8B == ï
                string friendlySaveName = Utils.GetOEMCodePageOrFallback(Encoding.UTF8).GetString(nameBuffer, 0, nameLength);

                string fileNameOnly = Path.GetFileName(fullPath);

                if (!TryGetGameSaveIndex(fileNameOnly, out ushort index))
                {
                    return false;
                }

                saveData = new SaveData(index, fullPath, fileNameOnly, friendlySaveName);
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
        else if (ushort.TryParse(fileNameOnly.AsSpan(4, 4), NumberStyles.None, NumberFormatInfo.InvariantInfo, out index) &&
                 index <= SaveSlotCount)
        {
            return true;
        }

        return false;
    }

    private static bool TrySeekToFriendlySaveName(Stream stream, out long friendlySaveNamePosition)
    {
        friendlySaveNamePosition = 0;

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
                friendlySaveNamePosition = stream.Position;
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }

    private static string GetFinalStoredSaveFileName(SaveData saveData)
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
                string finalDest = GetFinalStoredSaveFileName(saveData);
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
                string finalDest = GetFinalStoredSaveFileName(saveData);
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
            string finalDest = GetFinalStoredSaveFileName(saveData);
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

                SaveData? gameSaveData = Array.Find(InGameSaveDataList, x => x?.FileName.EqualsI(Path.GetFileName(tempDest)) == true);
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
        FillGameSaveDataList(Config.Thief2Path, InGameSaveDataList);
        View.RefreshInGameSavesList(InGameSaveDataList);
    }

    private static void RefreshViewStoredList()
    {
        FillStoredSaveDataList(Paths.SaveStore, StoredSaveDataList);
        View.RefreshSaveStoreList(StoredSaveDataList);
    }

    // TODO: Dedupe
    internal static bool RenameStoredSave(string newName)
    {
        if (ContainsInvalidFriendlySaveNameChars(newName))
        {
            return false;
        }

        // TODO: ASCII encoding for now, until we figure out how to convert to OEM which the game needs
        byte[] newNameBytes = Encoding.ASCII.GetBytes(newName);

        if (newNameBytes.Length > MaxFriendlySaveNameLength)
        {
            return false;
        }

        if (!View.TryGetSelectedStoredSaveIndex(out int index))
        {
            return false;
        }

        SaveData saveData = StoredSaveDataList[index];

        return RenameSave(saveData, newNameBytes);
    }

    // TODO: Dedupe
    internal static bool RenameGameSave(string newName)
    {
        if (ContainsInvalidFriendlySaveNameChars(newName))
        {
            return false;
        }

        // TODO: ASCII encoding for now, until we figure out how to convert to OEM which the game needs
        byte[] newNameBytes = Encoding.ASCII.GetBytes(newName);

        if (newNameBytes.Length > MaxFriendlySaveNameLength)
        {
            return false;
        }

        if (!View.TryGetSelectedInGameSaveIndex(out int index))
        {
            return false;
        }

        SaveData? saveData = InGameSaveDataList[index];
        if (saveData == null)
        {
            return false;
        }

        return RenameSave(saveData, newNameBytes);
    }

    private static bool RenameSave(SaveData saveData, byte[] newNameBytes_)
    {
        using (new DisableWatchers())
        {
            /*
            Must use a full-name-section-length array with the new name at the start and all the rest of the 
            bytes zero. Otherwise shorter names will still leave whatever was in there after their length (so
            "Cargo 0:00:00" -> "Cargo" becomes "Cargo 0:00:00" in the file still).
            */
            byte[] nameBytesFull = new byte[MaxFriendlySaveNameLength];
            Array.Copy(newNameBytes_, nameBytesFull, newNameBytes_.Length);

            try
            {
                bool seekSuccess;
                long friendlySaveNamePosition;
                using (FileStream fs = File.OpenRead(saveData.FullPath))
                {
                    seekSuccess = TrySeekToFriendlySaveName(fs, out friendlySaveNamePosition);
                }

                if (!seekSuccess) return false;

                using SafeFileHandle handle = File.OpenHandle(saveData.FullPath, FileMode.Open, FileAccess.Write);
                RandomAccess.Write(handle, nameBytesFull, friendlySaveNamePosition);

                return true;
            }
            // TODO: Report exception
            catch
            {
                return false;
            }
        }
    }

    /*
    TODO: Only allowing ASCII for now, because we need to convert to OEM code page but the field probably takes
     ANSI or UTF-16 or something.
    */
    internal static bool ContainsInvalidFriendlySaveNameChars(string value)
    {
        foreach (char c in value)
        {
            if (c < 32 || c >= 127)
            {
                return true;
            }
        }
        return false;
    }

    internal static bool TryGetSaveDataForSelectedGameSave([NotNullWhen(true)] out SaveData? saveData)
    {
        if (View.TryGetSelectedInGameSaveIndex(out int index))
        {
            saveData = InGameSaveDataList[index];
            return saveData != null;
        }

        saveData = null;
        return false;
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
