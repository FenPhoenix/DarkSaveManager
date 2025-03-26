using System.Diagnostics;

namespace DarkSaveManager;

public sealed partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void Test1Button_Click(object sender, EventArgs e)
    {
        List<SaveData> saveDataList = Core.GetSaveDataList(Config.Thief2Path);
        foreach (SaveData saveData in saveDataList)
        {
            Trace.WriteLine("SAVE ITEM:");
            Trace.WriteLine("----------");
            Trace.WriteLine(saveData.FileName);
            Trace.WriteLine(saveData.SaveName);
            Trace.WriteLine("");
            Trace.WriteLine("");
        }
    }

    internal void RefreshInGameSavesList(List<SaveData> saveDataList)
    {
        try
        {
            InGameSavesTreeView.BeginUpdate();
            InGameSavesTreeView.Nodes.Clear();
            foreach (SaveData saveData in saveDataList)
            {
                InGameSavesTreeView.Nodes.Add(saveData.SaveName);
            }
        }
        finally
        {
            InGameSavesTreeView.EndUpdate();
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        Core.Shutdown();
    }
}
