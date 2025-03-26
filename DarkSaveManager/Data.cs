namespace DarkSaveManager;

internal sealed class SaveData
{
    internal readonly int Index;
    internal readonly string FullPath;
    internal readonly string FileName;
    internal readonly string SaveName;

    internal SaveData(int index, string fullPath, string fileName, string saveName)
    {
        Index = index;
        FullPath = fullPath;
        FileName = fileName;
        SaveName = saveName;
    }
}
