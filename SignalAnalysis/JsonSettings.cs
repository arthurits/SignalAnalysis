using System.Text.Json;

namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Loads all settings from file <see cref="AppSettings.FileName"/> into variable <see cref="_settings"/>.
    /// Shows MessageBox error if unsuccessful
    /// </summary>
    private void LoadAppSettingsJSON()
    {
        try
        {
            var jsonString = File.ReadAllText(_settings.FileName);
            _settings = JsonSerializer.Deserialize<AppSettings>(jsonString) ?? _settings;
            //SetWindowPos(_settings.WindowPosition);
        }
        catch (FileNotFoundException)
        {
            _settingsFileExist = false;
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
    private void SaveAppSettingsJSON()
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
    /// Modifies window size and position to the values in <see cref="AppSettings">_settings</see>
    /// </summary>
    private void SetWindowPos()
    {
        this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
        this.DesktopLocation = new Point(_settings.WindowLeft, _settings.WindowTop);
        this.ClientSize = new Size(_settings.WindowWidth, _settings.WindowHeight);
    }
}
