using Microsoft.UI.Xaml;

namespace $safeprojectname$.Contracts.Services;

public interface IThemeSelectorService
{
    ElementTheme GetTheme();
    string GetThemeName();
    void SetTheme(ElementTheme theme);
}
