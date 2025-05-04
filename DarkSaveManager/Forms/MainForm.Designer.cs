using DarkSaveManager.Forms.CustomControls;

namespace DarkSaveManager.Forms;

sealed partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        Test1Button = new DarkButton();
        InGameSavesTreeView = new DarkTreeView();
        StoredSavesTreeView = new DarkTreeView();
        MoveToStoreButton = new DarkButton();
        CopyToStoreButton = new DarkButton();
        StoredSavesLabel = new DarkLabel();
        InGameSavesLabel = new DarkLabel();
        Test2Button = new DarkButton();
        SwapToGameButton = new DarkButton();
        ThiefGameTextBox = new DarkTextBox();
        GameSaveDirectoryLabel = new DarkLabel();
        ThiefGameBrowseButton = new DarkButton();
        RefreshButton = new DarkButton();
        StoredSaveDeleteButton = new DarkButton();
        EverythingPanel = new Panel();
        GamePathErrorPictureBox = new PictureBox();
        ListsHelpLabel = new DarkLabel();
        ListsPanel = new DrawnPanel();
        VisualThemeCheckBox = new DarkCheckBox();
        EverythingPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)GamePathErrorPictureBox).BeginInit();
        ListsPanel.SuspendLayout();
        SuspendLayout();
        // 
        // Test1Button
        // 
        Test1Button.Location = new Point(784, 88);
        Test1Button.Name = "Test1Button";
        Test1Button.Size = new Size(83, 23);
        Test1Button.TabIndex = 6;
        Test1Button.Text = "Test1";
        Test1Button.Click += Test1Button_Click;
        // 
        // InGameSavesTreeView
        // 
        InGameSavesTreeView.AllowDrop = true;
        InGameSavesTreeView.FullRowSelect = true;
        InGameSavesTreeView.HideSelection = false;
        InGameSavesTreeView.LabelEdit = true;
        InGameSavesTreeView.Location = new Point(448, 32);
        InGameSavesTreeView.Name = "InGameSavesTreeView";
        InGameSavesTreeView.ShowLines = false;
        InGameSavesTreeView.ShowPlusMinus = false;
        InGameSavesTreeView.ShowRootLines = false;
        InGameSavesTreeView.Size = new Size(305, 632);
        InGameSavesTreeView.TabIndex = 6;
        InGameSavesTreeView.AfterLabelEdit += InGameSavesTreeView_AfterLabelEdit;
        InGameSavesTreeView.ItemDrag += InGameSavesTreeView_ItemDrag;
        InGameSavesTreeView.AfterSelect += InGameSavesTreeView_AfterSelect;
        InGameSavesTreeView.DragDrop += InGameSavesTreeView_DragDrop;
        InGameSavesTreeView.DragOver += InGameSavesTreeView_DragOver;
        InGameSavesTreeView.KeyDown += TreeView_KeyDown;
        // 
        // StoredSavesTreeView
        // 
        StoredSavesTreeView.AllowDrop = true;
        StoredSavesTreeView.FullRowSelect = true;
        StoredSavesTreeView.HideSelection = false;
        StoredSavesTreeView.LabelEdit = true;
        StoredSavesTreeView.Location = new Point(0, 32);
        StoredSavesTreeView.Name = "StoredSavesTreeView";
        StoredSavesTreeView.ShowLines = false;
        StoredSavesTreeView.ShowPlusMinus = false;
        StoredSavesTreeView.ShowRootLines = false;
        StoredSavesTreeView.Size = new Size(305, 632);
        StoredSavesTreeView.TabIndex = 2;
        StoredSavesTreeView.AfterLabelEdit += StoredSavesTreeView_AfterLabelEdit;
        StoredSavesTreeView.ItemDrag += StoredSavesTreeView_ItemDrag;
        StoredSavesTreeView.AfterSelect += StoredSavesTreeView_AfterSelect;
        StoredSavesTreeView.DragDrop += StoredSavesTreeView_DragDrop;
        StoredSavesTreeView.DragOver += StoredSavesTreeView_DragOver;
        StoredSavesTreeView.KeyDown += TreeView_KeyDown;
        // 
        // MoveToStoreButton
        // 
        MoveToStoreButton.Enabled = false;
        MoveToStoreButton.Location = new Point(337, 160);
        MoveToStoreButton.Name = "MoveToStoreButton";
        MoveToStoreButton.Size = new Size(75, 23);
        MoveToStoreButton.TabIndex = 4;
        MoveToStoreButton.Text = "<- Move";
        MoveToStoreButton.Click += MoveToStoreButton_Click;
        // 
        // CopyToStoreButton
        // 
        CopyToStoreButton.Enabled = false;
        CopyToStoreButton.Location = new Point(337, 136);
        CopyToStoreButton.Name = "CopyToStoreButton";
        CopyToStoreButton.Size = new Size(75, 23);
        CopyToStoreButton.TabIndex = 3;
        CopyToStoreButton.Text = "<- Copy";
        CopyToStoreButton.Click += CopyToStoreButton_Click;
        // 
        // StoredSavesLabel
        // 
        StoredSavesLabel.AutoSize = true;
        StoredSavesLabel.Location = new Point(0, 8);
        StoredSavesLabel.Name = "StoredSavesLabel";
        StoredSavesLabel.Size = new Size(75, 15);
        StoredSavesLabel.TabIndex = 0;
        StoredSavesLabel.Text = "Stored saves:";
        // 
        // InGameSavesLabel
        // 
        InGameSavesLabel.AutoSize = true;
        InGameSavesLabel.Location = new Point(448, 8);
        InGameSavesLabel.Name = "InGameSavesLabel";
        InGameSavesLabel.Size = new Size(86, 15);
        InGameSavesLabel.TabIndex = 7;
        InGameSavesLabel.Text = "In-game saves:";
        // 
        // Test2Button
        // 
        Test2Button.Location = new Point(784, 112);
        Test2Button.Name = "Test2Button";
        Test2Button.Size = new Size(83, 23);
        Test2Button.TabIndex = 7;
        Test2Button.Text = "Test2";
        Test2Button.Click += Test2Button_Click;
        // 
        // SwapToGameButton
        // 
        SwapToGameButton.Enabled = false;
        SwapToGameButton.Location = new Point(337, 192);
        SwapToGameButton.Name = "SwapToGameButton";
        SwapToGameButton.Size = new Size(75, 23);
        SwapToGameButton.TabIndex = 5;
        SwapToGameButton.Text = "Swap ->";
        SwapToGameButton.Click += SwapToGameButton_Click;
        // 
        // ThiefGameTextBox
        // 
        ThiefGameTextBox.Location = new Point(16, 32);
        ThiefGameTextBox.Name = "ThiefGameTextBox";
        ThiefGameTextBox.Size = new Size(640, 23);
        ThiefGameTextBox.TabIndex = 1;
        ThiefGameTextBox.Leave += ThiefGameTextBox_Leave;
        // 
        // GameSaveDirectoryLabel
        // 
        GameSaveDirectoryLabel.AutoSize = true;
        GameSaveDirectoryLabel.Location = new Point(16, 16);
        GameSaveDirectoryLabel.Name = "GameSaveDirectoryLabel";
        GameSaveDirectoryLabel.Size = new Size(260, 15);
        GameSaveDirectoryLabel.TabIndex = 0;
        GameSaveDirectoryLabel.Text = "Game save directory (example: C:\\Thief2\\saves):";
        // 
        // ThiefGameBrowseButton
        // 
        ThiefGameBrowseButton.Location = new Point(656, 32);
        ThiefGameBrowseButton.Name = "ThiefGameBrowseButton";
        ThiefGameBrowseButton.Size = new Size(83, 23);
        ThiefGameBrowseButton.TabIndex = 2;
        ThiefGameBrowseButton.Text = "Browse...";
        ThiefGameBrowseButton.Click += ThiefGameBrowseButton_Click;
        // 
        // RefreshButton
        // 
        RefreshButton.Location = new Point(784, 48);
        RefreshButton.Name = "RefreshButton";
        RefreshButton.Size = new Size(83, 23);
        RefreshButton.TabIndex = 4;
        RefreshButton.Text = "Refresh";
        RefreshButton.Click += RefreshButton_Click;
        // 
        // StoredSaveDeleteButton
        // 
        StoredSaveDeleteButton.Location = new Point(283, 9);
        StoredSaveDeleteButton.Name = "StoredSaveDeleteButton";
        StoredSaveDeleteButton.Size = new Size(24, 23);
        StoredSaveDeleteButton.TabIndex = 1;
        StoredSaveDeleteButton.Text = "X";
        StoredSaveDeleteButton.Click += StoredSaveDeleteButton_Click;
        // 
        // EverythingPanel
        // 
        EverythingPanel.Controls.Add(GamePathErrorPictureBox);
        EverythingPanel.Controls.Add(ListsHelpLabel);
        EverythingPanel.Controls.Add(ListsPanel);
        EverythingPanel.Controls.Add(VisualThemeCheckBox);
        EverythingPanel.Controls.Add(Test1Button);
        EverythingPanel.Controls.Add(GameSaveDirectoryLabel);
        EverythingPanel.Controls.Add(ThiefGameTextBox);
        EverythingPanel.Controls.Add(Test2Button);
        EverythingPanel.Controls.Add(ThiefGameBrowseButton);
        EverythingPanel.Controls.Add(RefreshButton);
        EverythingPanel.Dock = DockStyle.Fill;
        EverythingPanel.Location = new Point(0, 0);
        EverythingPanel.Name = "EverythingPanel";
        EverythingPanel.Size = new Size(878, 792);
        EverythingPanel.TabIndex = 0;
        // 
        // GamePathErrorPictureBox
        // 
        GamePathErrorPictureBox.Location = new Point(744, 36);
        GamePathErrorPictureBox.Name = "GamePathErrorPictureBox";
        GamePathErrorPictureBox.Size = new Size(16, 16);
        GamePathErrorPictureBox.TabIndex = 13;
        GamePathErrorPictureBox.TabStop = false;
        GamePathErrorPictureBox.Visible = false;
        // 
        // ListsHelpLabel
        // 
        ListsHelpLabel.AutoSize = true;
        ListsHelpLabel.Location = new Point(208, 80);
        ListsHelpLabel.Name = "ListsHelpLabel";
        ListsHelpLabel.Size = new Size(382, 15);
        ListsHelpLabel.TabIndex = 12;
        ListsHelpLabel.Text = "Drag between the lists or use the buttons to move, copy, or swap saves.";
        // 
        // ListsPanel
        // 
        ListsPanel.Controls.Add(StoredSavesLabel);
        ListsPanel.Controls.Add(SwapToGameButton);
        ListsPanel.Controls.Add(CopyToStoreButton);
        ListsPanel.Controls.Add(StoredSaveDeleteButton);
        ListsPanel.Controls.Add(MoveToStoreButton);
        ListsPanel.Controls.Add(StoredSavesTreeView);
        ListsPanel.Controls.Add(InGameSavesLabel);
        ListsPanel.Controls.Add(InGameSavesTreeView);
        ListsPanel.Location = new Point(16, 104);
        ListsPanel.Name = "ListsPanel";
        ListsPanel.Size = new Size(760, 672);
        ListsPanel.TabIndex = 5;
        // 
        // VisualThemeCheckBox
        // 
        VisualThemeCheckBox.AutoSize = true;
        VisualThemeCheckBox.Location = new Point(784, 16);
        VisualThemeCheckBox.Name = "VisualThemeCheckBox";
        VisualThemeCheckBox.Size = new Size(84, 19);
        VisualThemeCheckBox.TabIndex = 3;
        VisualThemeCheckBox.Text = "Dark mode";
        VisualThemeCheckBox.CheckedChanged += VisualThemeCheckBox_CheckedChanged;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(878, 792);
        Controls.Add(EverythingPanel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "MainForm";
        ShowInTaskbar = true;
        Text = "Dark Save Manager";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        EverythingPanel.ResumeLayout(false);
        EverythingPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)GamePathErrorPictureBox).EndInit();
        ListsPanel.ResumeLayout(false);
        ListsPanel.PerformLayout();
        ResumeLayout(false);
    }

    #endregion

    private DarkButton Test1Button;
    private DarkTreeView InGameSavesTreeView;
    private DarkTreeView StoredSavesTreeView;
    private DarkButton MoveToStoreButton;
    private DarkButton CopyToStoreButton;
    private DarkLabel StoredSavesLabel;
    private DarkLabel InGameSavesLabel;
    private DarkButton Test2Button;
    private DarkButton SwapToGameButton;
    private DarkTextBox ThiefGameTextBox;
    private DarkLabel GameSaveDirectoryLabel;
    private DarkButton ThiefGameBrowseButton;
    private DarkButton RefreshButton;
    private DarkButton StoredSaveDeleteButton;
    private Panel EverythingPanel;
    private DarkCheckBox VisualThemeCheckBox;
    private DrawnPanel ListsPanel;
    private DarkLabel ListsHelpLabel;
    private PictureBox GamePathErrorPictureBox;
}
