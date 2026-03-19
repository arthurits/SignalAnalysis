using SignalAnalysis.Models;

namespace SignalAnalysis.Services;

public class TrayMenuOpeningEventArgs : EventArgs
{
    public List<TrayMenuItemDefinition> Items { get; } = [];
}
