using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkSaveManager;

internal static class Core
{
    internal static MainForm View = null!;

    internal static void Init()
    {
        View = new MainForm();
        View.Show();
    }

    internal static List<SaveData> GetSaveData(string gamePath)
    {

    }
}
