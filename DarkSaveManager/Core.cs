using System.Text;
using System.Text.RegularExpressions;

namespace DarkSaveManager;

internal static partial class Core
{
    internal static MainForm View = null!;

    internal static void Init()
    {
        View = new MainForm();
        View.Show();
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
            /*
            TODO: We only support ASCII names for now - can we find some clever way to detect?
             The T2 source code doesn't seem to explicitly do any encoding stuff at all - save names seem to be read
             in the OEM codepage (850 in my case): 8B == ï
            */
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

                return (true, Encoding.ASCII.GetString(nameBuffer, 0, nameLength));
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
}
