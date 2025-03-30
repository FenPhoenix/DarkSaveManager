namespace DarkSaveManager;

internal sealed class SaveData
{
    internal readonly int Index;
    internal readonly string FullPath;
    internal readonly string FileName;
    internal readonly string FriendlySaveName;

    internal SaveData(int index, string fullPath, string fileName, string friendlySaveName)
    {
        Index = index;
        FullPath = fullPath;
        FileName = fileName;
        FriendlySaveName = friendlySaveName;
    }
}
