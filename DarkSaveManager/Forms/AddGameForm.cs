namespace DarkSaveManager.Forms;

public sealed partial class AddGameForm : DarkFormBase
{
    public readonly Game Result = new();

    public AddGameForm()
    {
        InitializeComponent();
    }

    public override void RespondToSystemThemeChange()
    {
        SetThemeBase(Config.VisualTheme);
    }

    private void GameSaveDirectoryBrowseButton_Click(object sender, EventArgs e)
    {
        using FolderBrowserDialog d = new();
        if (d.ShowDialogDark(this) != DialogResult.OK) return;
        GameSaveDirectoryTextBox.Text = d.SelectedPath;

        CheckValidity();
    }

    private void CheckValidity()
    {
        bool valid = !GameNameTextBox.Text.IsWhiteSpace() &&
                     Directory.Exists(GameSaveDirectoryTextBox.Text) &&
                     StoredNameIsValid();

        OKButton.Enabled = valid;

        if (valid)
        {
            Result.Name = GameNameTextBox.Text;
            Result.GameSavesPath = GameSaveDirectoryTextBox.Text;
            Result.StoredSavesDirName = StoredSavesDirectoryNameTextBox.Text;
        }

        return;

        bool StoredNameIsValid()
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in StoredSavesDirectoryNameTextBox.Text)
            {
                if (invalidChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
    }

    private void TextBoxes_Leave(object sender, EventArgs e)
    {
        CheckValidity();
    }
}
