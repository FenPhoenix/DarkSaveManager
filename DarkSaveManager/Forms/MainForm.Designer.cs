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
        RefreshButton = new DarkButton();
        StoredSaveDeleteButton = new DarkButton();
        GamesGroupBox = new DarkGroupBox();
        RemoveGameButton = new DarkButton();
        EditGameButton = new DarkButton();
        AddGameButton = new DarkButton();
        GamesComboBox = new DarkComboBox();
        EverythingPanel = new Panel();
        VisualThemeCheckBox = new DarkCheckBox();
        GamesGroupBox.SuspendLayout();
        EverythingPanel.SuspendLayout();
        SuspendLayout();
        // 
        // Test1Button
        // 
        Test1Button.Location = new Point(784, 40);
        Test1Button.Name = "Test1Button";
        Test1Button.Size = new Size(83, 23);
        Test1Button.TabIndex = 0;
        Test1Button.Text = "Test1";
        Test1Button.Click += Test1Button_Click;
        // 
        // InGameSavesTreeView
        // 
        InGameSavesTreeView.AllowDrop = true;
        InGameSavesTreeView.FullRowSelect = true;
        InGameSavesTreeView.HideSelection = false;
        InGameSavesTreeView.LabelEdit = true;
        InGameSavesTreeView.Location = new Point(504, 224);
        InGameSavesTreeView.Name = "InGameSavesTreeView";
        InGameSavesTreeView.ShowLines = false;
        InGameSavesTreeView.ShowPlusMinus = false;
        InGameSavesTreeView.ShowRootLines = false;
        InGameSavesTreeView.Size = new Size(248, 464);
        InGameSavesTreeView.TabIndex = 1;
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
        StoredSavesTreeView.Location = new Point(56, 224);
        StoredSavesTreeView.Name = "StoredSavesTreeView";
        StoredSavesTreeView.ShowLines = false;
        StoredSavesTreeView.ShowPlusMinus = false;
        StoredSavesTreeView.ShowRootLines = false;
        StoredSavesTreeView.Size = new Size(248, 464);
        StoredSavesTreeView.TabIndex = 1;
        StoredSavesTreeView.AfterLabelEdit += StoredSavesTreeView_AfterLabelEdit;
        StoredSavesTreeView.ItemDrag += StoredSavesTreeView_ItemDrag;
        StoredSavesTreeView.DragDrop += StoredSavesTreeView_DragDrop;
        StoredSavesTreeView.DragOver += StoredSavesTreeView_DragOver;
        StoredSavesTreeView.KeyDown += TreeView_KeyDown;
        // 
        // MoveToStoreButton
        // 
        MoveToStoreButton.Enabled = false;
        MoveToStoreButton.Location = new Point(368, 368);
        MoveToStoreButton.Name = "MoveToStoreButton";
        MoveToStoreButton.Size = new Size(75, 23);
        MoveToStoreButton.TabIndex = 2;
        MoveToStoreButton.Text = "<- Move";
        MoveToStoreButton.Click += MoveToStoreButton_Click;
        // 
        // CopyToStoreButton
        // 
        CopyToStoreButton.Enabled = false;
        CopyToStoreButton.Location = new Point(368, 344);
        CopyToStoreButton.Name = "CopyToStoreButton";
        CopyToStoreButton.Size = new Size(75, 23);
        CopyToStoreButton.TabIndex = 2;
        CopyToStoreButton.Text = "<- Copy";
        CopyToStoreButton.Click += CopyToStoreButton_Click;
        // 
        // StoredSavesLabel
        // 
        StoredSavesLabel.AutoSize = true;
        StoredSavesLabel.Location = new Point(56, 208);
        StoredSavesLabel.Name = "StoredSavesLabel";
        StoredSavesLabel.Size = new Size(75, 15);
        StoredSavesLabel.TabIndex = 3;
        StoredSavesLabel.Text = "Stored saves:";
        // 
        // InGameSavesLabel
        // 
        InGameSavesLabel.AutoSize = true;
        InGameSavesLabel.Location = new Point(504, 208);
        InGameSavesLabel.Name = "InGameSavesLabel";
        InGameSavesLabel.Size = new Size(86, 15);
        InGameSavesLabel.TabIndex = 3;
        InGameSavesLabel.Text = "In-game saves:";
        // 
        // Test2Button
        // 
        Test2Button.Location = new Point(784, 64);
        Test2Button.Name = "Test2Button";
        Test2Button.Size = new Size(83, 23);
        Test2Button.TabIndex = 0;
        Test2Button.Text = "Test2";
        Test2Button.Click += Test2Button_Click;
        // 
        // SwapToGameButton
        // 
        SwapToGameButton.Enabled = false;
        SwapToGameButton.Location = new Point(368, 400);
        SwapToGameButton.Name = "SwapToGameButton";
        SwapToGameButton.Size = new Size(75, 23);
        SwapToGameButton.TabIndex = 2;
        SwapToGameButton.Text = "Swap ->";
        SwapToGameButton.Click += SwapToGameButton_Click;
        // 
        // RefreshButton
        // 
        RefreshButton.Location = new Point(784, 104);
        RefreshButton.Name = "RefreshButton";
        RefreshButton.Size = new Size(83, 23);
        RefreshButton.TabIndex = 7;
        RefreshButton.Text = "Refresh";
        RefreshButton.Click += RefreshButton_Click;
        // 
        // StoredSaveDeleteButton
        // 
        StoredSaveDeleteButton.Location = new Point(281, 200);
        StoredSaveDeleteButton.Name = "StoredSaveDeleteButton";
        StoredSaveDeleteButton.Size = new Size(24, 23);
        StoredSaveDeleteButton.TabIndex = 8;
        StoredSaveDeleteButton.Text = "X";
        StoredSaveDeleteButton.Click += StoredSaveDeleteButton_Click;
        // 
        // GamesGroupBox
        // 
        GamesGroupBox.Controls.Add(RemoveGameButton);
        GamesGroupBox.Controls.Add(EditGameButton);
        GamesGroupBox.Controls.Add(AddGameButton);
        GamesGroupBox.Controls.Add(GamesComboBox);
        GamesGroupBox.Location = new Point(16, 16);
        GamesGroupBox.Name = "GamesGroupBox";
        GamesGroupBox.Size = new Size(752, 144);
        GamesGroupBox.TabIndex = 9;
        GamesGroupBox.TabStop = false;
        GamesGroupBox.Text = "Games";
        // 
        // RemoveGameButton
        // 
        RemoveGameButton.Location = new Point(656, 32);
        RemoveGameButton.Name = "RemoveGameButton";
        RemoveGameButton.Size = new Size(80, 23);
        RemoveGameButton.TabIndex = 8;
        RemoveGameButton.Text = "Remove...";
        RemoveGameButton.Click += RemoveGameButton_Click;
        // 
        // EditGameButton
        // 
        EditGameButton.Location = new Point(592, 32);
        EditGameButton.Name = "EditGameButton";
        EditGameButton.Size = new Size(64, 23);
        EditGameButton.TabIndex = 8;
        EditGameButton.Text = "Edit...";
        EditGameButton.Click += EditGameButton_Click;
        // 
        // AddGameButton
        // 
        AddGameButton.Location = new Point(536, 32);
        AddGameButton.Name = "AddGameButton";
        AddGameButton.Size = new Size(56, 23);
        AddGameButton.TabIndex = 8;
        AddGameButton.Text = "Add...";
        AddGameButton.Click += AddGameButton_Click;
        // 
        // GamesComboBox
        // 
        GamesComboBox.FormattingEnabled = true;
        GamesComboBox.Location = new Point(16, 32);
        GamesComboBox.Name = "GamesComboBox";
        GamesComboBox.Size = new Size(520, 23);
        GamesComboBox.TabIndex = 7;
        GamesComboBox.SelectedIndexChanged += GamesComboBox_SelectedIndexChanged;
        // 
        // EverythingPanel
        // 
        EverythingPanel.Controls.Add(VisualThemeCheckBox);
        EverythingPanel.Controls.Add(GamesGroupBox);
        EverythingPanel.Controls.Add(Test1Button);
        EverythingPanel.Controls.Add(StoredSaveDeleteButton);
        EverythingPanel.Controls.Add(Test2Button);
        EverythingPanel.Controls.Add(RefreshButton);
        EverythingPanel.Controls.Add(InGameSavesTreeView);
        EverythingPanel.Controls.Add(InGameSavesLabel);
        EverythingPanel.Controls.Add(StoredSavesTreeView);
        EverythingPanel.Controls.Add(StoredSavesLabel);
        EverythingPanel.Controls.Add(MoveToStoreButton);
        EverythingPanel.Controls.Add(CopyToStoreButton);
        EverythingPanel.Controls.Add(SwapToGameButton);
        EverythingPanel.Dock = DockStyle.Fill;
        EverythingPanel.Location = new Point(0, 0);
        EverythingPanel.Name = "EverythingPanel";
        EverythingPanel.Size = new Size(881, 771);
        EverythingPanel.TabIndex = 10;
        // 
        // VisualThemeCheckBox
        // 
        VisualThemeCheckBox.AutoSize = true;
        VisualThemeCheckBox.Location = new Point(784, 16);
        VisualThemeCheckBox.Name = "VisualThemeCheckBox";
        VisualThemeCheckBox.Size = new Size(84, 19);
        VisualThemeCheckBox.TabIndex = 10;
        VisualThemeCheckBox.Text = "Dark mode";
        VisualThemeCheckBox.CheckedChanged += VisualThemeCheckBox_CheckedChanged;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(881, 771);
        Controls.Add(EverythingPanel);
        Name = "MainForm";
        ShowInTaskbar = true;
        Text = "Dark Save Manager";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        GamesGroupBox.ResumeLayout(false);
        EverythingPanel.ResumeLayout(false);
        EverythingPanel.PerformLayout();
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
    private DarkButton RefreshButton;
    private DarkButton StoredSaveDeleteButton;
    private DarkGroupBox GamesGroupBox;
    private DarkComboBox GamesComboBox;
    private DarkButton RemoveGameButton;
    private DarkButton EditGameButton;
    private DarkButton AddGameButton;
    private Panel EverythingPanel;
    private DarkCheckBox VisualThemeCheckBox;
}
