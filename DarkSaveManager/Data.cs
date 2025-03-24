namespace DarkSaveManager;

internal sealed class SaveData
{
    internal readonly string FullPath;
    internal readonly string FileName;
    internal readonly string SaveName;

    internal SaveData(string fullPath, string fileName, string saveName)
    {
        FullPath = fullPath;
        FileName = fileName;
        SaveName = saveName;
    }
}
