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
        List<SaveData> saveDataList = Core.GetSaveData(@"C:\Thief Games\Thief-ND-TFix2-127a (Titanium Practice)");
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
}
