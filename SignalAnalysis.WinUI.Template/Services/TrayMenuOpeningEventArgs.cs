using SignalAnalysis.Template.Models;

namespace SignalAnalysis.Template.Services;

public class TrayMenuOpeningEventArgs : EventArgs
{
    public List<TrayMenuItemDefinition> Items { get; } = [];
}
