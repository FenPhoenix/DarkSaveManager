namespace DarkSaveManager;

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
        Test1Button = new Button();
        InGameSavesTreeView = new TreeView();
        StoredSavesTreeView = new TreeView();
        MoveToStoreButton = new Button();
        CopyToStoreButton = new Button();
        StoredSavesLabel = new Label();
        InGameSavesLabel = new Label();
        Test2Button = new Button();
        SwapToGameButton = new Button();
        ThiefGameTextBox = new TextBox();
        GameSaveDirectoryLabel = new Label();
        ThiefGameBrowseButton = new Button();
        RefreshButton = new Button();
        StoredSaveDeleteButton = new Button();
        GamesGroupBox = new GroupBox();
        GamesComboBox = new ComboBox();
        AddGameButton = new Button();
        EditGameButton = new Button();
        RemoveGameButton = new Button();
        GamesGroupBox.SuspendLayout();
        SuspendLayout();
        // 
        // Test1Button
        // 
        Test1Button.Location = new Point(792, 8);
        Test1Button.Name = "Test1Button";
        Test1Button.Size = new Size(75, 23);
        Test1Button.TabIndex = 0;
        Test1Button.Text = "Test1";
        Test1Button.UseVisualStyleBackColor = true;
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
        MoveToStoreButton.UseVisualStyleBackColor = true;
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
        CopyToStoreButton.UseVisualStyleBackColor = true;
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
        Test2Button.Location = new Point(792, 32);
        Test2Button.Name = "Test2Button";
        Test2Button.Size = new Size(75, 23);
        Test2Button.TabIndex = 0;
        Test2Button.Text = "Test2";
        Test2Button.UseVisualStyleBackColor = true;
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
        SwapToGameButton.UseVisualStyleBackColor = true;
        SwapToGameButton.Click += SwapToGameButton_Click;
        // 
        // ThiefGameTextBox
        // 
        ThiefGameTextBox.Location = new Point(16, 96);
        ThiefGameTextBox.Name = "ThiefGameTextBox";
        ThiefGameTextBox.Size = new Size(648, 23);
        ThiefGameTextBox.TabIndex = 4;
        ThiefGameTextBox.Leave += ThiefGameTextBox_Leave;
        // 
        // GameSaveDirectoryLabel
        // 
        GameSaveDirectoryLabel.AutoSize = true;
        GameSaveDirectoryLabel.Location = new Point(16, 80);
        GameSaveDirectoryLabel.Name = "GameSaveDirectoryLabel";
        GameSaveDirectoryLabel.Size = new Size(117, 15);
        GameSaveDirectoryLabel.TabIndex = 5;
        GameSaveDirectoryLabel.Text = "Game save directory:";
        // 
        // ThiefGameBrowseButton
        // 
        ThiefGameBrowseButton.Location = new Point(664, 96);
        ThiefGameBrowseButton.Name = "ThiefGameBrowseButton";
        ThiefGameBrowseButton.Size = new Size(83, 23);
        ThiefGameBrowseButton.TabIndex = 6;
        ThiefGameBrowseButton.Text = "Browse...";
        ThiefGameBrowseButton.UseVisualStyleBackColor = true;
        ThiefGameBrowseButton.Click += ThiefGameBrowseButton_Click;
        // 
        // RefreshButton
        // 
        RefreshButton.Location = new Point(792, 72);
        RefreshButton.Name = "RefreshButton";
        RefreshButton.Size = new Size(75, 23);
        RefreshButton.TabIndex = 7;
        RefreshButton.Text = "Refresh";
        RefreshButton.UseVisualStyleBackColor = true;
        RefreshButton.Click += RefreshButton_Click;
        // 
        // StoredSaveDeleteButton
        // 
        StoredSaveDeleteButton.Location = new Point(281, 200);
        StoredSaveDeleteButton.Name = "StoredSaveDeleteButton";
        StoredSaveDeleteButton.Size = new Size(24, 23);
        StoredSaveDeleteButton.TabIndex = 8;
        StoredSaveDeleteButton.Text = "X";
        StoredSaveDeleteButton.UseVisualStyleBackColor = true;
        StoredSaveDeleteButton.Click += StoredSaveDeleteButton_Click;
        // 
        // GamesGroupBox
        // 
        GamesGroupBox.Controls.Add(RemoveGameButton);
        GamesGroupBox.Controls.Add(EditGameButton);
        GamesGroupBox.Controls.Add(AddGameButton);
        GamesGroupBox.Controls.Add(GamesComboBox);
        GamesGroupBox.Controls.Add(GameSaveDirectoryLabel);
        GamesGroupBox.Controls.Add(ThiefGameTextBox);
        GamesGroupBox.Controls.Add(ThiefGameBrowseButton);
        GamesGroupBox.Location = new Point(16, 16);
        GamesGroupBox.Name = "GamesGroupBox";
        GamesGroupBox.Size = new Size(760, 144);
        GamesGroupBox.TabIndex = 9;
        GamesGroupBox.TabStop = false;
        GamesGroupBox.Text = "Games";
        // 
        // GamesComboBox
        // 
        GamesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        GamesComboBox.FormattingEnabled = true;
        GamesComboBox.Location = new Point(16, 32);
        GamesComboBox.Name = "GamesComboBox";
        GamesComboBox.Size = new Size(528, 23);
        GamesComboBox.TabIndex = 7;
        // 
        // AddGameButton
        // 
        AddGameButton.Location = new Point(544, 32);
        AddGameButton.Name = "AddGameButton";
        AddGameButton.Size = new Size(56, 23);
        AddGameButton.TabIndex = 8;
        AddGameButton.Text = "Add...";
        AddGameButton.UseVisualStyleBackColor = true;
        // 
        // EditGameButton
        // 
        EditGameButton.Location = new Point(600, 32);
        EditGameButton.Name = "EditGameButton";
        EditGameButton.Size = new Size(64, 23);
        EditGameButton.TabIndex = 8;
        EditGameButton.Text = "Edit...";
        EditGameButton.UseVisualStyleBackColor = true;
        // 
        // RemoveGameButton
        // 
        RemoveGameButton.Location = new Point(664, 32);
        RemoveGameButton.Name = "RemoveGameButton";
        RemoveGameButton.Size = new Size(80, 23);
        RemoveGameButton.TabIndex = 8;
        RemoveGameButton.Text = "Remove...";
        RemoveGameButton.UseVisualStyleBackColor = true;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(881, 771);
        Controls.Add(GamesGroupBox);
        Controls.Add(StoredSaveDeleteButton);
        Controls.Add(RefreshButton);
        Controls.Add(InGameSavesLabel);
        Controls.Add(StoredSavesLabel);
        Controls.Add(CopyToStoreButton);
        Controls.Add(SwapToGameButton);
        Controls.Add(MoveToStoreButton);
        Controls.Add(StoredSavesTreeView);
        Controls.Add(InGameSavesTreeView);
        Controls.Add(Test2Button);
        Controls.Add(Test1Button);
        Name = "MainForm";
        Text = "Dark Save Manager";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        GamesGroupBox.ResumeLayout(false);
        GamesGroupBox.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button Test1Button;
    private TreeView InGameSavesTreeView;
    private TreeView StoredSavesTreeView;
    private Button MoveToStoreButton;
    private Button CopyToStoreButton;
    private Label StoredSavesLabel;
    private Label InGameSavesLabel;
    private Button Test2Button;
    private Button SwapToGameButton;
    private TextBox ThiefGameTextBox;
    private Label GameSaveDirectoryLabel;
    private Button ThiefGameBrowseButton;
    private Button RefreshButton;
    private Button StoredSaveDeleteButton;
    private GroupBox GamesGroupBox;
    private ComboBox GamesComboBox;
    private Button RemoveGameButton;
    private Button EditGameButton;
    private Button AddGameButton;
}
