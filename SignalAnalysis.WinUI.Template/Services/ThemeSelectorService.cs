using Microsoft.UI.Xaml;
using $safeprojectname$;
using $safeprojectname$.Contracts.Services;
using $safeprojectname$.Helpers;

namespace $safeprojectname$.Services;

public class ThemeSelectorService : IThemeSelectorService
{
    public ElementTheme GetTheme()
    {
        if (App.MainWindow.Content is FrameworkElement frameworkElement)
        {
            return frameworkElement.ActualTheme;
        }
        return ElementTheme.Default;
    }

    public string GetThemeName()
    {
        return GetTheme().ToString();
    }

    public void SetTheme(ElementTheme theme)
    {
        if (App.MainWindow.Content is FrameworkElement frameworkElement)
        {
            frameworkElement.RequestedTheme = theme;

            TitleBarHelper.UpdateTitleBar(theme);
        }
    }
}