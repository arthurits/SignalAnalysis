using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Loads all settings from file _sett.FileName into class instance _settings
    /// Shows MessageBox error if unsuccessful
    /// </summary>
    private void LoadProgramSettingsJSON()
    {
        try
        {
            var jsonString = File.ReadAllText(_settings.FileName);
            _settings = JsonSerializer.Deserialize<ClassSettings>(jsonString) ?? _settings;
            
            //this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            //this.DesktopLocation = new Point(_settings.Wnd_Left, _settings.Wnd_Top);
            //this.ClientSize = new Size(_settings.Wnd_Width, _settings.Wnd_Height);
        }
        catch (FileNotFoundException)
        {
        }
        catch (Exception ex)
        {
            using (new CenterWinDialog(this))
            {
                MessageBox.Show(this,
                    String.Format(StringsRM.GetString("strErrorDeserialize") ?? string.Empty ,ex.Message),
                    StringsRM.GetString("strErrorDeserializeTitle"),
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
        //_settings.Wnd_Left = DesktopLocation.X;
        //_settings.Wnd_Top = DesktopLocation.Y;
        //_settings.Wnd_Width = ClientSize.Width;
        //_settings.Wnd_Height = ClientSize.Height;

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        var jsonString = JsonSerializer.Serialize(_settings, options);
        File.WriteAllText(_settings.FileName, jsonString);
    }

}

