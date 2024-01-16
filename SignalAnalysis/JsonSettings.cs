using System.Text.Json;

namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Loads all settings from file <see cref="AppSettings.FileName"/> into variable <see cref="_settings"/>.
    /// Shows MessageBox error if unsuccessful
    /// </summary>
    private void LoadProgramSettingsJSON()
    {
        try
        {
            var jsonString = File.ReadAllText(_settings.FileName);
            _settings = JsonSerializer.Deserialize<AppSettings>(jsonString) ?? _settings;

            ApplySettingsJSON(_settings.WindowPosition);
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception ex)
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    string.Format(StringResources.ErrorDeserialize, ex.Message),
                    StringResources.ErrorDeserializeTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

    }

    /// <summary>
    /// Saves data from class instance _sett into _sett.FileName
    /// </summary>
    private void SaveProgramSettingsJSON()
    {
        _settings.WindowLeft = DesktopLocation.X;
        _settings.WindowTop = DesktopLocation.Y;
        _settings.WindowWidth = ClientSize.Width;
        _settings.WindowHeight = ClientSize.Height;

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(_settings, options);
        File.WriteAllText(_settings.FileName, jsonString);
    }

    /// <summary>
    /// Update UI with settings
    /// </summary>
    /// <param name="WindowPosition"><see langword="True"/> if the window position and size should be applied. <see langword="False"/> if omitted</param>
    private void ApplySettingsJSON(bool WindowPosition = false)
    {
        if (WindowPosition)
        {
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.DesktopLocation = new Point(_settings.WindowLeft, _settings.WindowTop);
            this.ClientSize = new Size(_settings.WindowWidth, _settings.WindowHeight);
        }
    }
}
