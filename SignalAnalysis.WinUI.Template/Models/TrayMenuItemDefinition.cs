namespace $safeprojectname$.Models;

public class TrayMenuItemDefinition
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsSeparator { get; set; } = false;
    public bool IsEnabled { get; set; } = true;
    public string? IconPath { get; set; }
    public List<TrayMenuItemDefinition> Children { get; } = [];
}

public enum TrayMenuItemId
{
    Open = 0,
    Exit = 1,
}
