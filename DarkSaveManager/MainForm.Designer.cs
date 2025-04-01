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
        ThiefGameLabel = new Label();
        ThiefGameBrowseButton = new Button();
        RefreshButton = new Button();
        StoredSaveDeleteButton = new Button();
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
        InGameSavesTreeView.Location = new Point(504, 96);
        InGameSavesTreeView.Name = "InGameSavesTreeView";
        InGameSavesTreeView.ShowLines = false;
        InGameSavesTreeView.ShowPlusMinus = false;
        InGameSavesTreeView.ShowRootLines = false;
        InGameSavesTreeView.Size = new Size(248, 464);
        InGameSavesTreeView.TabIndex = 1;
        InGameSavesTreeView.AfterLabelEdit += InGameSavesTreeView_AfterLabelEdit;
        InGameSavesTreeView.AfterSelect += InGameSavesTreeView_AfterSelect;
        InGameSavesTreeView.DragDrop += InGameSavesTreeView_DragDrop;
        InGameSavesTreeView.DragEnter += InGameSavesTreeView_DragEnter;
        InGameSavesTreeView.DragOver += InGameSavesTreeView_DragOver;
        InGameSavesTreeView.KeyDown += TreeView_KeyDown;
        // 
        // StoredSavesTreeView
        // 
        StoredSavesTreeView.FullRowSelect = true;
        StoredSavesTreeView.HideSelection = false;
        StoredSavesTreeView.LabelEdit = true;
        StoredSavesTreeView.Location = new Point(56, 96);
        StoredSavesTreeView.Name = "StoredSavesTreeView";
        StoredSavesTreeView.ShowLines = false;
        StoredSavesTreeView.ShowPlusMinus = false;
        StoredSavesTreeView.ShowRootLines = false;
        StoredSavesTreeView.Size = new Size(248, 464);
        StoredSavesTreeView.TabIndex = 1;
        StoredSavesTreeView.AfterLabelEdit += StoredSavesTreeView_AfterLabelEdit;
        StoredSavesTreeView.ItemDrag += StoredSavesTreeView_ItemDrag;
        StoredSavesTreeView.KeyDown += TreeView_KeyDown;
        // 
        // MoveToStoreButton
        // 
        MoveToStoreButton.Enabled = false;
        MoveToStoreButton.Location = new Point(368, 240);
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
        CopyToStoreButton.Location = new Point(368, 216);
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
        StoredSavesLabel.Location = new Point(56, 80);
        StoredSavesLabel.Name = "StoredSavesLabel";
        StoredSavesLabel.Size = new Size(75, 15);
        StoredSavesLabel.TabIndex = 3;
        StoredSavesLabel.Text = "Stored saves:";
        // 
        // InGameSavesLabel
        // 
        InGameSavesLabel.AutoSize = true;
        InGameSavesLabel.Location = new Point(504, 80);
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
        SwapToGameButton.Location = new Point(368, 272);
        SwapToGameButton.Name = "SwapToGameButton";
        SwapToGameButton.Size = new Size(75, 23);
        SwapToGameButton.TabIndex = 2;
        SwapToGameButton.Text = "Swap ->";
        SwapToGameButton.UseVisualStyleBackColor = true;
        SwapToGameButton.Click += SwapToGameButton_Click;
        // 
        // ThiefGameTextBox
        // 
        ThiefGameTextBox.Location = new Point(16, 656);
        ThiefGameTextBox.Name = "ThiefGameTextBox";
        ThiefGameTextBox.Size = new Size(776, 23);
        ThiefGameTextBox.TabIndex = 4;
        ThiefGameTextBox.Leave += ThiefGameTextBox_Leave;
        // 
        // ThiefGameLabel
        // 
        ThiefGameLabel.AutoSize = true;
        ThiefGameLabel.Location = new Point(16, 640);
        ThiefGameLabel.Name = "ThiefGameLabel";
        ThiefGameLabel.Size = new Size(70, 15);
        ThiefGameLabel.TabIndex = 5;
        ThiefGameLabel.Text = "Thief game:";
        // 
        // ThiefGameBrowseButton
        // 
        ThiefGameBrowseButton.Location = new Point(792, 656);
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
        StoredSaveDeleteButton.Location = new Point(281, 72);
        StoredSaveDeleteButton.Name = "StoredSaveDeleteButton";
        StoredSaveDeleteButton.Size = new Size(24, 23);
        StoredSaveDeleteButton.TabIndex = 8;
        StoredSaveDeleteButton.Text = "X";
        StoredSaveDeleteButton.UseVisualStyleBackColor = true;
        StoredSaveDeleteButton.Click += StoredSaveDeleteButton_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(881, 771);
        Controls.Add(StoredSaveDeleteButton);
        Controls.Add(RefreshButton);
        Controls.Add(ThiefGameBrowseButton);
        Controls.Add(ThiefGameLabel);
        Controls.Add(ThiefGameTextBox);
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
    private Label ThiefGameLabel;
    private Button ThiefGameBrowseButton;
    private Button RefreshButton;
    private Button StoredSaveDeleteButton;
}
