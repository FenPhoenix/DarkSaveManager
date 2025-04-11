using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using DarkSaveManager.Forms.CustomControls;
using DarkSaveManager.Forms.WinFormsNative;
using Microsoft.VisualBasic.FileIO;

namespace DarkSaveManager.Forms;

public sealed partial class MainForm : DarkFormBase, IEventDisabler, IMessageFilter
{
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int EventsDisabled { get; set; }

    // Stupid hack for if event handlers need to know
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    internal bool StartupState { get; private set; } = true;

    private readonly IDarkable[] _lazyLoadedControls;

    public MainForm()
    {
        _lazyLoadedControls = Array.Empty<IDarkable>();

        InitializeComponent();

        SetTheme(Config.VisualTheme, startup: true, createControlHandles: true);

        using (new DisableEvents(this))
        {
            VisualThemeCheckBox.Checked = Config.DarkMode;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        Application.AddMessageFilter(this);
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        StartupState = false;
    }

    protected override void WndProc(ref Message m)
    {
        if (!StartupState)
        {
            if (m.Msg == Native.WM_THEMECHANGED)
            {
                Win32ThemeHooks.ReloadTheme();
            }
            else if (ControlUtils.SystemThemeHasChanged(ref m, out VisualTheme newTheme))
            {
                Config.VisualTheme = newTheme;
                SetTheme(Config.VisualTheme);
                m.Result = IntPtr.Zero;

                List<IntPtr> handles = Native.GetProcessWindowHandles();
                foreach (IntPtr handle in handles)
                {
                    Control? control = Control.FromHandle(handle);
                    if (control is DarkFormBase form) form.RespondToSystemThemeChange();
                }
            }
        }
        base.WndProc(ref m);
    }

    private bool ModalDialogUp() => !CanFocus;

    internal bool CursorOverControl(Control control, bool fullArea = false)
    {
        if (!control.Visible || !control.Enabled) return false;

        Point rpt = PointToClient(control.PointToScreen(Point.Empty));
        Size rcs = fullArea ? control.Size : control.ClientSize;

        Point ptc = this.ClientCursorPos();

        // Don't create eleventy billion Rectangle objects per second
        return ptc.X >= rpt.X && ptc.X < rpt.X + rcs.Width &&
               ptc.Y >= rpt.Y && ptc.Y < rpt.Y + rcs.Height;
    }

    public bool PreFilterMessage(ref Message m)
    {
        const bool BlockMessage = true;
        const bool PassMessageOn = false;

        static bool TryGetHWndFromMousePos(Message msg, out IntPtr result, [NotNullWhen(true)] out Control? control)
        {
            Point pos = new(Native.SignedLOWORD(msg.LParam), Native.SignedHIWORD(msg.LParam));
            result = Native.WindowFromPoint(pos);
            control = Control.FromHandle(result);
            return control != null;
        }

        // This allows controls to be scrolled with the mousewheel when the mouse is over them, without
        // needing to actually be focused. Vital for a good user experience.

        #region Mouse

        if (m.Msg == Native.WM_MOUSEWHEEL)
        {
            // IMPORTANT (PreFilterMessage):
            // Do this check inside each if block rather than above, because the message may not
            // be a mousemove message, and in that case we'd be trying to get a window point from a random
            // value, and that causes the min,max,close button flickering.
            if (!TryGetHWndFromMousePos(m, out IntPtr hWnd, out Control? controlOver)) return PassMessageOn;

            if (controlOver is DarkComboBox { SuppressScrollWheelValueChange: true, Focused: false } cb)
            {
                if (cb.Parent is { IsHandleCreated: true })
                {
                    Native.SendMessageW(cb.Parent.Handle, m.Msg, m.WParam, m.LParam);
                }
                else
                {
                    return BlockMessage;
                }
            }
            else
            {
                Native.SendMessageW(hWnd, m.Msg, m.WParam, m.LParam);
            }
            return BlockMessage;
        }
        else if (m.Msg == Native.WM_MOUSEHWHEEL)
        {
            if (!TryGetHWndFromMousePos(m, out _, out _)) return PassMessageOn;
        }
        // NC = Non-Client, ie. the mouse was in a non-client area of the control
        else if (m.Msg is Native.WM_MOUSEMOVE or Native.WM_NCMOUSEMOVE)
        {
            if (ModalDialogUp()) return PassMessageOn;

            Control? control = Control.FromHandle(Native.WindowFromPoint(Cursor.Position));
            if (control is ToolStripDropDown) return PassMessageOn;
        }
        else if (m.Msg is
                 Native.WM_LBUTTONDOWN or Native.WM_NCLBUTTONDOWN or
                 Native.WM_MBUTTONDOWN or Native.WM_NCMBUTTONDOWN or
                 Native.WM_RBUTTONDOWN or Native.WM_NCRBUTTONDOWN or
                 Native.WM_LBUTTONDBLCLK or Native.WM_NCLBUTTONDBLCLK or
                 Native.WM_MBUTTONDBLCLK or Native.WM_NCMBUTTONDBLCLK or
                 Native.WM_RBUTTONDBLCLK or Native.WM_NCRBUTTONDBLCLK or
                 Native.WM_LBUTTONUP or Native.WM_NCLBUTTONUP or
                 Native.WM_MBUTTONUP or Native.WM_NCMBUTTONUP or
                 Native.WM_RBUTTONUP or Native.WM_NCRBUTTONUP)
        {
            if (ModalDialogUp()) return PassMessageOn;
        }
        #endregion
        #region Keys
        // To handle alt presses, we have to handle WM_SYSKEYDOWN, which handles alt and F10. Sure why not.
        else if (m.Msg is Native.WM_SYSKEYDOWN or Native.WM_SYSKEYUP)
        {
            int wParam = (int)m.WParam;
            if (ModifierKeys == Keys.Alt && wParam == (int)Keys.F4)
            {
                return PassMessageOn;
            }
        }
        // Any other keys have to use this.
        else if (m.Msg == Native.WM_KEYDOWN)
        {
        }
        else if (m.Msg == Native.WM_KEYUP)
        {
        }
        #endregion

        return PassMessageOn;
    }

    public override void RespondToSystemThemeChange() => SetTheme(Config.VisualTheme);

    public void SetTheme(VisualTheme theme) => SetTheme(theme, startup: false, createControlHandles: false);

    private void SetTheme(VisualTheme theme, bool startup, bool createControlHandles)
    {
        bool darkMode = theme == VisualTheme.Dark;

        try
        {
            if (!startup) EverythingPanel.SuspendDrawing();

            if (startup && !darkMode)
            {
                ControlUtils.CreateAllControlsHandles(
                    control: this,
                    createHandlePredicate: static _ => true);
            }
            else
            {
                SetThemeBase(
                    theme: theme,
                    excludePredicate: static x => x is SplitterPanel,
                    createControlHandles: createControlHandles,
                    createHandlePredicate: static _ => true,
                    capacity: 150
                );
            }

            if (!startup) ControlUtils.RecreateAllToolTipHandles();

            if (!startup || darkMode)
            {
                foreach (IDarkable lazyLoadedControl in _lazyLoadedControls)
                {
                    lazyLoadedControl.DarkModeEnabled = darkMode;
                }
            }
        }
        finally
        {
            if (!startup) EverythingPanel.ResumeDrawing();
        }
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
                InGameSavesTreeView.Nodes.Add(saveData?.FriendlySaveName ?? "< EMPTY >");
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
        Application.RemoveMessageFilter(this);
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

    private static bool IsSameList(DragEventArgs e, TreeView treeView)
    {
        if (e.Data?.GetData(typeof(TreeNode)) is not TreeNode node ||
            node.TreeView == treeView)
        {
            e.Effect = DragDropEffects.None;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void StoredSavesTreeView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        if (e.Item == null) return;

        DoDragDrop(e.Item, DragDropEffects.Move);
    }

    private void InGameSavesTreeView_DragOver(object sender, DragEventArgs e)
    {
        if (IsSameList(e, InGameSavesTreeView)) return;

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
        if (IsSameList(e, InGameSavesTreeView)) return;

        Point pt = InGameSavesTreeView.PointToClient(new Point(e.X, e.Y));
        TreeViewHitTestInfo info = InGameSavesTreeView.HitTest(pt);
        if (info.Node != null && e.Data?.GetData(typeof(TreeNode)) is TreeNode node)
        {
            Core.SwapSaveToGame_DragDrop(node.Index, info.Node.Index);
        }
    }

    private void InGameSavesTreeView_ItemDrag(object sender, ItemDragEventArgs e)
    {
        if (e.Button != MouseButtons.Left) return;
        if (e.Item == null) return;

        DoDragDrop(e.Item, DragDropEffects.Move | DragDropEffects.Copy);
    }

    private void StoredSavesTreeView_DragOver(object sender, DragEventArgs e)
    {
        if (IsSameList(e, StoredSavesTreeView)) return;

        e.Effect = KeyStateIsCtrl(e.KeyState) ? DragDropEffects.Copy : DragDropEffects.Move;
    }

    private void StoredSavesTreeView_DragDrop(object sender, DragEventArgs e)
    {
        if (IsSameList(e, StoredSavesTreeView)) return;

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

    private void UpdateGamePath()
    {
        Config.GamePath = ThiefGameTextBox.Text;
        ConfigIni.WriteIni();
        Core.RefreshGamePath();
        Core.RefreshViewAllLists();
    }

    private void ThiefGameBrowseButton_Click(object sender, EventArgs e)
    {
        using FolderBrowserDialog d = new();
        if (d.ShowDialogDark(this) != DialogResult.OK) return;
        ThiefGameTextBox.Text = d.SelectedPath;
        UpdateGamePath();
    }

    private void ThiefGameTextBox_Leave(object sender, EventArgs e)
    {
        UpdateGamePath();
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

    // TODO: Make this a nicer looking standard "sun/moon" light/dark switch
    private void VisualThemeCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        if (EventsDisabled > 0) return;

        Config.VisualTheme = VisualThemeCheckBox.Checked ? VisualTheme.Dark : VisualTheme.Classic;
        SetTheme(Config.VisualTheme);
    }
}
