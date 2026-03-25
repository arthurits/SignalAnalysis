namespace $safeprojectname$.Models;

public class CultureOption
{
    public string DisplayName { get; set; } = string.Empty;
    public string LanguageTag { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"{DisplayName} ({LanguageTag})";
    }
}
