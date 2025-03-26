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
        SwappedOutSavesTreeView = new TreeView();
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
        InGameSavesTreeView.Location = new Point(56, 96);
        InGameSavesTreeView.Name = "InGameSavesTreeView";
        InGameSavesTreeView.Size = new Size(248, 464);
        InGameSavesTreeView.TabIndex = 1;
        // 
        // SwappedOutSavesTreeView
        // 
        SwappedOutSavesTreeView.Location = new Point(512, 96);
        SwappedOutSavesTreeView.Name = "SwappedOutSavesTreeView";
        SwappedOutSavesTreeView.Size = new Size(248, 464);
        SwappedOutSavesTreeView.TabIndex = 1;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(881, 771);
        Controls.Add(SwappedOutSavesTreeView);
        Controls.Add(InGameSavesTreeView);
        Controls.Add(Test1Button);
        Name = "MainForm";
        Text = "Dark Save Manager";
        FormClosing += MainForm_FormClosing;
        FormClosed += MainForm_FormClosed;
        ResumeLayout(false);
    }

    #endregion

    private Button Test1Button;
    private TreeView InGameSavesTreeView;
    private TreeView SwappedOutSavesTreeView;
}
