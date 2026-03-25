using Microsoft.UI.Xaml;
using SignalAnalysis.Template;
using SignalAnalysis.Template.Contracts.Services;
using SignalAnalysis.Template.Helpers;

namespace SignalAnalysis.Template.Services;

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