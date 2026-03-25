using $safeprojectname$.Models;

namespace $safeprojectname$.Services;

public class TrayMenuOpeningEventArgs : EventArgs
{
    public List<TrayMenuItemDefinition> Items { get; } = [];
}
