using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using DarkSaveManager.Forms;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace DarkSaveManager;

// TODO: Make UI properly blank/disable etc. when game path is invalid/blank/nonexistent
// TODO: Fill out personal post-build copy bat file

internal static class Core
{
    internal static MainForm View = null!;

    internal static readonly SaveData?[] InGameSaveDataList = new SaveData?[SaveSlotCount];
    internal static readonly List<SaveData> StoredSaveDataList = new();

    // TODO: Implement list of them, one for each game path
    internal static readonly FileSystemWatcher GameWatcher = new();
    internal static readonly FileSystemWatcher SaveStoreWatcher = new();

    internal static void RefreshGamePath()
    {
        try
        {
            GameWatcher.Path = Config.GamePath;
            GameWatcher.EnableRaisingEvents = true;
        }
        catch
        {
            // Directory doesn't exist
        }
    }

    internal static void Init()
    {
        Directory.CreateDirectory(Paths.SaveStore);
        Directory.CreateDirectory(Paths.Temp);

        ConfigIni.ReadIni();

        GameWatcher.Filter = "*.sav";
        GameWatcher.NotifyFilter =
            NotifyFilters.LastWrite
            | NotifyFilters.CreationTime
            | NotifyFilters.FileName;
        GameWatcher.Changed += GameWatcher_Changed;
        GameWatcher.Created += GameWatcher_Changed;
        GameWatcher.Deleted += GameWatcher_Changed;
        GameWatcher.Renamed += GameWatcher_Changed;

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

        View.UpdateGamesList();

        UpdatePaths();

        try
        {
            GameWatcher.EnableRaisingEvents = true;
        }
        catch
        {
            // Dir doesn't exist; hold off till game path refresh
        }

        SaveStoreWatcher.EnableRaisingEvents = true;
    }

    private static void UpdatePaths()
    {
        using (new DisableWatchers())
        {
            try
            {
                GameWatcher.Path = Config.GamePath;
            }
            catch
            {
                // Dir doesn't exist; hold off till game path refresh
            }

            SaveStoreWatcher.Path = Paths.SaveStore;
        }
    }

    private static void GameWatcher_Changed(object sender, FileSystemEventArgs e)
    {
        UpdateGameSaveDataList(e.FullPath);
        View.Invoke(static () => View.RefreshInGameSavesList(InGameSaveDataList));
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
        foreach (string saveFile in saveFiles)
        {
            if (TryGetSaveData(saveFile, out SaveData? saveData))
            {
                saveDataList.Add(saveData);
            }
        }
    }

    internal static void UpdateGameSaveDataList(string saveFile)
    {
        if (TryGetSaveData(saveFile, out SaveData? saveData))
        {
            InGameSaveDataList[saveData.SlotIndex] = saveData;
        }
        else
        {
            ReadOnlySpan<char> fileName = Path.GetFileName(saveFile.AsSpan());
            if (TryGetGameSaveIndex(fileName, out ushort index))
            {
                InGameSaveDataList[index] = null;
            }
            else if (fileName.EqualsI(QuickSaveFileName))
            {
                InGameSaveDataList[QuickSaveIndex] = null;
            }
        }
    }

    internal static void FillGameSaveDataList(string savePath, SaveData?[] saveDataList)
    {
        Array.Clear(saveDataList);

        if (!Directory.Exists(savePath)) return;

        const string pattern = "*.sav";

        string[] saveFiles = Directory.GetFiles(savePath, pattern);
        foreach (string saveFile in saveFiles)
        {
            if (TryGetSaveData(saveFile, out SaveData? saveData))
            {
                saveDataList[saveData.SlotIndex] = saveData;
            }
        }
    }

    private static bool TryGetSaveData(string fullPath, [NotNullWhen(true)] out SaveData? saveData)
    {
        saveData = null;

        try
        {
            using FileStream stream = File.OpenRead(fullPath);

            if (!TrySeekToFriendlySaveName(stream, out _)) return false;

            using RentScope<byte> nameBuffer = new(MaxFriendlySaveNameLength);

            stream.ReadExactly(nameBuffer.Span);

            int nameEndIndex = nameBuffer.Span.IndexOf((byte)0);
            int nameLength = nameEndIndex == -1 ? MaxFriendlySaveNameLength : nameEndIndex + 1;

            // The T2 source code doesn't seem to explicitly do any encoding stuff at all - save names seem
            // to be read in the OEM codepage (850 in my case): 8B == ï
            string friendlySaveName = Utils.GetOEMCodePageOrFallback(Encoding.UTF8).GetString(nameBuffer.Span[..nameLength]);

            string fileNameOnly = Path.GetFileName(fullPath);

            if (!TryGetGameSaveIndex(fileNameOnly, out ushort index))
            {
                return false;
            }

            saveData = new SaveData(index, fullPath, fileNameOnly, friendlySaveName);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryGetGameSaveIndex(ReadOnlySpan<char> fileNameOnly, out ushort index)
    {
        if (fileNameOnly.StartsWithI(QuickSaveFileName))
        {
            index = QuickSaveIndex;
            return true;
        }
        else if (fileNameOnly.Length >= 12 &&
                 fileNameOnly.StartsWithI("game") &&
                 fileNameOnly[8] == '.' &&
                 ushort.TryParse(fileNameOnly.Slice(4, 4), NumberStyles.None, NumberFormatInfo.InvariantInfo, out index) &&
                 index < QuickSaveIndex)
        {
            return true;
        }

        index = 0;
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

                if (!ChunkHeaderIsSaveDesc(chunkHeaderBuffer))
                {
                    continue;
                }

                stream.Position = offset;
                stream.ReadExactly(chunkHeaderBuffer);

                if (!ChunkHeaderIsSaveDesc(chunkHeaderBuffer))
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool ChunkHeaderIsSaveDesc(Span<byte> chunkHeaderBuffer)
        {
            return chunkHeaderBuffer.SequenceEqual("SAVEDESC\0\0\0\0"u8);
        }
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
                SaveData? saveData = InGameSaveDataList[index];
                if (saveData == null) return;
                string finalDest = GetFinalStoredSaveFileName(saveData);
                File.Copy(saveData.FullPath, finalDest);

                RefreshViewAllLists();
            }
        }
    }

    // TODO: Handle errors and if it already exists
    internal static void CopySaveDataToStore(int index)
    {
        using (new DisableWatchers())
        {
            SaveData? saveData = InGameSaveDataList[index];
            if (saveData == null) return;

            // TODO: Validate
            string finalDest = GetFinalStoredSaveFileName(saveData);
            File.Copy(saveData.FullPath, finalDest);

            RefreshViewAllLists();
        }
    }

    // TODO: Handle errors and if it already exists
    internal static void MoveSaveDataToStore(int index)
    {
        using (new DisableWatchers())
        {
            SaveData? saveData = InGameSaveDataList[index];
            if (saveData == null) return;

            // TODO: Validate
            string finalDest = GetFinalStoredSaveFileName(saveData);
            File.Move(saveData.FullPath, finalDest);

            RefreshViewAllLists();
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
                SaveData? saveData = InGameSaveDataList[index];
                if (saveData == null) return;
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

    internal static void SwapSaveToGame_DragDrop(int storedIndex, int gameIndex)
    {
        if (storedIndex == -1) return;
        if (gameIndex is < 0 or > QuickSaveIndex) return;

        using (new DisableWatchers())
        {
            // TODO: Validate
            SaveData storedSaveData = StoredSaveDataList[storedIndex];
            string tempDest = Path.Combine(Paths.Temp, storedSaveData.FileName.Substring(0, storedSaveData.FileName.LastIndexOf('_')));

            // TODO: Overwrite or notify or?
            File.Move(storedSaveData.FullPath, tempDest, overwrite: true);

            SaveData? gameSaveData = InGameSaveDataList[gameIndex];
            if (gameSaveData != null)
            {
                MoveToStore(gameSaveData);
            }

            string destFileName = gameIndex == QuickSaveIndex
                ? QuickSaveFileName
                : "game" + gameIndex.ToStrInv().PadLeft(4, '0') + ".sav";

            string gameDest = Path.Combine(Config.GamePath, destFileName);
            File.Move(tempDest, gameDest);

            RefreshViewAllLists();
        }
    }

    internal static void SwapSaveToGame(int slotIndex = -1)
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

                string destFileName = Path.GetFileName(tempDest);
                SaveData? gameSaveData = Array.Find(InGameSaveDataList, x => x?.FileName.EqualsI(destFileName) == true);
                if (gameSaveData != null)
                {
                    MoveToStore(gameSaveData);
                }

                if (slotIndex is > -1 and < QuickSaveIndex)
                {
                    if (TryGetGameSaveIndex(destFileName, out ushort destFileNameIndex))
                    {
                        destFileName = "game" + destFileNameIndex.ToStrInv().PadLeft(4, '0') + ".sav";
                    }
                }

                string gameDest = Path.Combine(Config.GamePath, destFileName);
                File.Move(tempDest, gameDest);

                RefreshViewAllLists();
            }
        }
    }

    internal static void RefreshViewAllLists()
    {
        RefreshViewInGameList();
        RefreshViewStoredList();
    }

    private static void RefreshViewInGameList()
    {
        FillGameSaveDataList(Config.GamePath, InGameSaveDataList);
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

    private static bool RenameSave(SaveData saveData, byte[] newNameBytes)
    {
        using (new DisableWatchers())
        {
            /*
            Must use a full-name-section-length array with the new name at the start and all the rest of the 
            bytes zero. Otherwise shorter names will still leave whatever was in there after their length (so
            "Cargo 0:00:00" -> "Cargo" becomes "Cargo 0:00:00" in the file still).
            */
            using RentScope<byte> nameBytesFull = new(MaxFriendlySaveNameLength, clear: true);
            newNameBytes.CopyTo(nameBytesFull.Span);

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
                RandomAccess.Write(handle, nameBytesFull.Span, friendlySaveNamePosition);

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

    internal static bool TryGetSaveDataForSelectedStoredSave([NotNullWhen(true)] out SaveData? saveData)
    {
        if (View.TryGetSelectedStoredSaveIndex(out int index))
        {
            saveData = StoredSaveDataList[index];
            return true;
        }

        saveData = null;
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

    internal static void Shutdown()
    {
        ConfigIni.WriteIni();
        Application.Exit();
    }

    private sealed class DisableWatchers : IDisposable
    {
        private static readonly Lock _lock = new();
        private static int _count;

        private readonly bool _oldGameEventWatchingValue;
        private readonly bool _oldStoreEventWatchingValue;

        public DisableWatchers()
        {
            lock (_lock)
            {
                if (_count == 0)
                {
                    _oldGameEventWatchingValue = GameWatcher.EnableRaisingEvents;
                    _oldStoreEventWatchingValue = SaveStoreWatcher.EnableRaisingEvents;

                    GameWatcher.EnableRaisingEvents = false;
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
                    GameWatcher.EnableRaisingEvents = _oldGameEventWatchingValue;
                    SaveStoreWatcher.EnableRaisingEvents = _oldStoreEventWatchingValue;
                }
                _count--;
            }
        }
    }

    internal static VisualTheme GetSystemTheme()
    {
        try
        {
            // Firefox uses this registry key, so if it's reliable enough for them, it's reliable enough for me
            object? appsUseLightThemeKey = Registry.GetValue(
                keyName: @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize",
                valueName: "AppsUseLightTheme",
                defaultValue: "");

            if (appsUseLightThemeKey is int keyInt)
            {
                return keyInt == 0 ? VisualTheme.Dark : VisualTheme.Classic;
            }
        }
        catch
        {
            return VisualTheme.Classic;
        }

        return VisualTheme.Classic;
    }

    internal static void OpenLogFile()
    {
        try
        {
            Utils.ProcessStart_UseShellExecute(Paths.LogFile);
        }
        catch
        {
            View.ShowAlert($"Unable to open log file.{NL}{NL}" + Paths.LogFile, LText.AlertMessages.Error);
        }
    }

    internal static void AddGame(Game game)
    {
        Config.Games.Add(game);
        if (Config.Games.Count == 1)
        {
            Config.CurrentGame = Config.Games[0];
        }
        View.UpdateGamesList();
    }
}
