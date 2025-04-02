namespace DarkSaveManager;

internal sealed class SaveData
{
    internal readonly int SlotIndex;
    internal readonly string FullPath;
    internal readonly string FileName;
    internal readonly string FriendlySaveName;

    internal SaveData(int slotIndex, string fullPath, string fileName, string friendlySaveName)
    {
        SlotIndex = slotIndex;
        FullPath = fullPath;
        FileName = fileName;
        FriendlySaveName = friendlySaveName;
    }
}
