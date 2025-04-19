namespace DarkSaveManager.Forms;

partial class AddGameForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        GameSaveDirectoryLabel = new DarkSaveManager.Forms.CustomControls.DarkLabel();
        GameSaveDirectoryTextBox = new DarkSaveManager.Forms.CustomControls.DarkTextBox();
        GameSaveDirectoryBrowseButton = new DarkSaveManager.Forms.CustomControls.DarkButton();
        GameNameTextBox = new DarkSaveManager.Forms.CustomControls.DarkTextBox();
        GameNameLabel = new DarkSaveManager.Forms.CustomControls.DarkLabel();
        StoredSavesDirectoryNameTextBox = new DarkSaveManager.Forms.CustomControls.DarkTextBox();
        StoredSavesDirectoryNameLabel = new DarkSaveManager.Forms.CustomControls.DarkLabel();
        BottomButtonsFLP = new DarkSaveManager.Forms.CustomControls.DarkFlowLayoutPanel();
        Cancel_Button = new DarkSaveManager.Forms.CustomControls.StandardButton();
        OKButton = new DarkSaveManager.Forms.CustomControls.StandardButton();
        BottomButtonsFLP.SuspendLayout();
        SuspendLayout();
        // 
        // GameSaveDirectoryLabel
        // 
        GameSaveDirectoryLabel.AutoSize = true;
        GameSaveDirectoryLabel.Location = new Point(40, 80);
        GameSaveDirectoryLabel.Name = "GameSaveDirectoryLabel";
        GameSaveDirectoryLabel.Size = new Size(117, 15);
        GameSaveDirectoryLabel.TabIndex = 8;
        GameSaveDirectoryLabel.Text = "Game save directory:";
        // 
        // GameSaveDirectoryTextBox
        // 
        GameSaveDirectoryTextBox.Location = new Point(40, 96);
        GameSaveDirectoryTextBox.Name = "GameSaveDirectoryTextBox";
        GameSaveDirectoryTextBox.Size = new Size(640, 23);
        GameSaveDirectoryTextBox.TabIndex = 7;
        GameSaveDirectoryTextBox.Leave += TextBoxes_Leave;
        // 
        // GameSaveDirectoryBrowseButton
        // 
        GameSaveDirectoryBrowseButton.Location = new Point(680, 96);
        GameSaveDirectoryBrowseButton.Name = "GameSaveDirectoryBrowseButton";
        GameSaveDirectoryBrowseButton.Size = new Size(83, 23);
        GameSaveDirectoryBrowseButton.TabIndex = 9;
        GameSaveDirectoryBrowseButton.Text = "Browse...";
        GameSaveDirectoryBrowseButton.Click += GameSaveDirectoryBrowseButton_Click;
        // 
        // GameNameTextBox
        // 
        GameNameTextBox.Location = new Point(40, 40);
        GameNameTextBox.Name = "GameNameTextBox";
        GameNameTextBox.Size = new Size(640, 23);
        GameNameTextBox.TabIndex = 7;
        GameNameTextBox.Leave += TextBoxes_Leave;
        // 
        // GameNameLabel
        // 
        GameNameLabel.AutoSize = true;
        GameNameLabel.Location = new Point(40, 24);
        GameNameLabel.Name = "GameNameLabel";
        GameNameLabel.Size = new Size(74, 15);
        GameNameLabel.TabIndex = 8;
        GameNameLabel.Text = "Game name:";
        // 
        // StoredSavesDirectoryNameTextBox
        // 
        StoredSavesDirectoryNameTextBox.Location = new Point(40, 152);
        StoredSavesDirectoryNameTextBox.Name = "StoredSavesDirectoryNameTextBox";
        StoredSavesDirectoryNameTextBox.Size = new Size(640, 23);
        StoredSavesDirectoryNameTextBox.TabIndex = 7;
        StoredSavesDirectoryNameTextBox.Leave += TextBoxes_Leave;
        // 
        // StoredSavesDirectoryNameLabel
        // 
        StoredSavesDirectoryNameLabel.AutoSize = true;
        StoredSavesDirectoryNameLabel.Location = new Point(40, 136);
        StoredSavesDirectoryNameLabel.Name = "StoredSavesDirectoryNameLabel";
        StoredSavesDirectoryNameLabel.Size = new Size(158, 15);
        StoredSavesDirectoryNameLabel.TabIndex = 8;
        StoredSavesDirectoryNameLabel.Text = "Stored saves directory name:";
        // 
        // BottomButtonsFLP
        // 
        BottomButtonsFLP.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        BottomButtonsFLP.Controls.Add(Cancel_Button);
        BottomButtonsFLP.Controls.Add(OKButton);
        BottomButtonsFLP.FlowDirection = FlowDirection.RightToLeft;
        BottomButtonsFLP.Location = new Point(0, 218);
        BottomButtonsFLP.Name = "BottomButtonsFLP";
        BottomButtonsFLP.Size = new Size(800, 32);
        BottomButtonsFLP.TabIndex = 10;
        // 
        // Cancel_Button
        // 
        Cancel_Button.DialogResult = DialogResult.Cancel;
        Cancel_Button.Location = new Point(722, 3);
        Cancel_Button.Name = "Cancel_Button";
        Cancel_Button.TabIndex = 0;
        Cancel_Button.Text = "Cancel";
        // 
        // OKButton
        // 
        OKButton.DialogResult = DialogResult.OK;
        OKButton.Enabled = false;
        OKButton.Location = new Point(641, 3);
        OKButton.Name = "OKButton";
        OKButton.TabIndex = 0;
        OKButton.Text = "OK";
        // 
        // AddGameForm
        // 
        AcceptButton = OKButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = Cancel_Button;
        ClientSize = new Size(800, 250);
        Controls.Add(BottomButtonsFLP);
        Controls.Add(GameNameLabel);
        Controls.Add(GameNameTextBox);
        Controls.Add(StoredSavesDirectoryNameLabel);
        Controls.Add(StoredSavesDirectoryNameTextBox);
        Controls.Add(GameSaveDirectoryLabel);
        Controls.Add(GameSaveDirectoryTextBox);
        Controls.Add(GameSaveDirectoryBrowseButton);
        Name = "AddGameForm";
        Text = "AddGameForm";
        BottomButtonsFLP.ResumeLayout(false);
        BottomButtonsFLP.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private CustomControls.DarkLabel GameSaveDirectoryLabel;
    private CustomControls.DarkTextBox GameSaveDirectoryTextBox;
    private CustomControls.DarkButton GameSaveDirectoryBrowseButton;
    private CustomControls.DarkTextBox GameNameTextBox;
    private CustomControls.DarkLabel GameNameLabel;
    private CustomControls.DarkTextBox StoredSavesDirectoryNameTextBox;
    private CustomControls.DarkLabel StoredSavesDirectoryNameLabel;
    private CustomControls.DarkFlowLayoutPanel BottomButtonsFLP;
    private CustomControls.StandardButton Cancel_Button;
    private CustomControls.StandardButton OKButton;
}