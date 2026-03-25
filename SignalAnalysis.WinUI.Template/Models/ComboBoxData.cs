namespace SignalAnalysis.Template.Models;

public class ComboBoxData
{
    // Text to be shown to the user in the UI
    public string DisplayName { get; set; } = string.Empty;
    
    // The value used in the code to handle the selection
    public int Value { get; set; } = 0;
}
