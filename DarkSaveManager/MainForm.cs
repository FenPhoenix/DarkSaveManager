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
        Core.FillSaveDataList(Config.Thief2Path, Core.InGameSaveDataList, stored: false);
        foreach (SaveData saveData in Core.InGameSaveDataList)
        {
            Trace.WriteLine("SAVE ITEM:");
            Trace.WriteLine("----------");
            Trace.WriteLine(saveData.FileName);
            Trace.WriteLine(saveData.FriendlySaveName);
            Trace.WriteLine("");
            Trace.WriteLine("");
        }
    }

    private void Test2Button_Click(object sender, EventArgs e)
    {
    }

    private static void RefreshList(TreeView treeView, List<SaveData> saveDataList, bool stored)
    {
        try
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();
            foreach (SaveData saveData in saveDataList)
            {
                treeView.Nodes.Add(saveData.FriendlySaveName);
            }
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    internal void RefreshInGameSavesList(List<SaveData> saveDataList)
    {
        RefreshList(InGameSavesTreeView, saveDataList, stored: false);
    }

    internal void RefreshSaveStoreList(List<SaveData> saveDataList)
    {
        RefreshList(StoredSavesTreeView, saveDataList, stored: true);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
    }

    private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        Core.Shutdown();
    }

    private void CopyToStoreButton_Click(object sender, EventArgs e)
    {
        Core.CopySelectedToStore();
    }

    private void MoveToStoreButton_Click(object sender, EventArgs e)
    {
        Core.MoveSelectedToStore();
    }

    private void SwapToGameButton_Click(object sender, EventArgs e)
    {
        Core.SwapSaveToGame();
    }

    internal bool TryGetSelectedInGameSaveIndex(out int index)
    {
        if (InGameSavesTreeView.SelectedNode != null)
        {
            index = InGameSavesTreeView.SelectedNode.Index;
            return true;
        }
        else
        {
            index = -1;
            return false;
        }
    }

    internal bool TryGetSelectedStoredSaveIndex(out int index)
    {
        if (StoredSavesTreeView.SelectedNode != null)
        {
            index = StoredSavesTreeView.SelectedNode.Index;
            return true;
        }
        else
        {
            index = -1;
            return false;
        }
    }

    private void StoredSavesTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
        if (e.CancelEdit || e.Label == null)
        {
            e.CancelEdit = true;
            return;
        }

        if (!Core.RenameStoredSave(e.Label))
        {
            e.CancelEdit = true;
        }
    }

    private void InGameSavesTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
    {
        if (e.CancelEdit || e.Label == null)
        {
            e.CancelEdit = true;
            return;
        }

        if (!Core.RenameGameSave(e.Label))
        {
            e.CancelEdit = true;
        }
    }

    private void TreeView_KeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TreeView treeView) return;

        if (e.KeyCode == Keys.F2 && treeView.SelectedNode != null)
        {
            treeView.SelectedNode.BeginEdit();
        }
    }
}
