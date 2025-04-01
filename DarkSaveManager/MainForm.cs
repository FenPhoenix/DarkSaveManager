using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;

namespace DarkSaveManager;

public sealed partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void Test1Button_Click(object sender, EventArgs e)
    {
    }

    private void Test2Button_Click(object sender, EventArgs e)
    {
    }

    internal void RefreshInGameSavesList(SaveData?[] saveDataList)
    {
        try
        {
            InGameSavesTreeView.BeginUpdate();
            InGameSavesTreeView.Nodes.Clear();
            foreach (SaveData? saveData in saveDataList)
            {
                InGameSavesTreeView.Nodes.Add(saveData != null ? saveData.FriendlySaveName : "< EMPTY >");
            }
        }
        finally
        {
            InGameSavesTreeView.EndUpdate();
        }
    }

    internal void RefreshSaveStoreList(List<SaveData> saveDataList)
    {
        try
        {
            StoredSavesTreeView.BeginUpdate();
            StoredSavesTreeView.Nodes.Clear();
            foreach (SaveData saveData in saveDataList)
            {
                StoredSavesTreeView.Nodes.Add(saveData.FriendlySaveName);
            }
        }
        finally
        {
            StoredSavesTreeView.EndUpdate();
        }
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

    private void InGameSavesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (Core.TryGetSaveDataForSelectedGameSave(out _))
        {
            CopyToStoreButton.Enabled = true;
            MoveToStoreButton.Enabled = true;
        }
        else
        {
            CopyToStoreButton.Enabled = false;
            MoveToStoreButton.Enabled = false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool KeyStateIsCtrl(int keyState) => (keyState & 8) == 8;

    private void StoredSavesTreeView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        if (e.Item != null)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }
    }

    private void InGameSavesTreeView_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = DragDropEffects.Move;
        Point pt = InGameSavesTreeView.PointToClient(new Point(e.X, e.Y));
        TreeViewHitTestInfo info = InGameSavesTreeView.HitTest(pt);
        if (info.Node != null)
        {
            InGameSavesTreeView.SelectedNode = info.Node;
        }
    }

    private void InGameSavesTreeView_DragDrop(object sender, DragEventArgs e)
    {
        Point pt = InGameSavesTreeView.PointToClient(new Point(e.X, e.Y));
        TreeViewHitTestInfo info = InGameSavesTreeView.HitTest(pt);
        if (info.Node != null && e.Data?.GetData(typeof(TreeNode)) is TreeNode node)
        {
            Core.SwapSaveToGame_DragDrop(node.Index, info.Node.Index);
        }
    }

    private void InGameSavesTreeView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        if (e.Item != null)
        {
            DoDragDrop(e.Item, DragDropEffects.Move | DragDropEffects.Copy);
        }
    }

    private void StoredSavesTreeView_DragOver(object sender, DragEventArgs e)
    {
        e.Effect = KeyStateIsCtrl(e.KeyState) ? DragDropEffects.Copy : DragDropEffects.Move;
    }

    private void StoredSavesTreeView_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetData(typeof(TreeNode)) is TreeNode node)
        {
            if (KeyStateIsCtrl(e.KeyState))
            {
                Core.CopySaveDataToStore(node.Index);
            }
            else
            {
                Core.MoveSaveDataToStore(node.Index);
            }
        }
    }

    private void ThiefGameBrowseButton_Click(object sender, EventArgs e)
    {
        using FolderBrowserDialog d = new();
        DialogResult result = d.ShowDialog();
        if (result != DialogResult.OK) return;
        ThiefGameTextBox.Text = d.SelectedPath;

        Config.GamePath = ThiefGameTextBox.Text;
        ConfigIni.WriteIni();
        Core.RefreshGamePath();
        Core.RefreshViewAllLists();
    }

    private void ThiefGameTextBox_Leave(object sender, EventArgs e)
    {
        Config.GamePath = ThiefGameTextBox.Text;
        ConfigIni.WriteIni();
        Core.RefreshGamePath();
        Core.RefreshViewAllLists();
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
        Core.RefreshViewAllLists();
    }

    internal void SetGamePathField(string gamePath)
    {
        ThiefGameTextBox.Text = gamePath;
    }

    private void StoredSaveDeleteButton_Click(object sender, EventArgs e)
    {
        if (Core.TryGetSaveDataForSelectedStoredSave(out SaveData? saveData))
        {
            FileSystem.DeleteFile(
                saveData.FullPath,
                UIOption.OnlyErrorDialogs,
                RecycleOption.SendToRecycleBin);

            Core.RefreshViewAllLists();
        }
    }
}
