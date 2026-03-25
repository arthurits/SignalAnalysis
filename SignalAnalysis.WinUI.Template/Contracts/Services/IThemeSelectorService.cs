using Microsoft.UI.Xaml;

namespace SignalAnalysis.Contracts.Services;

public interface IThemeSelectorService
{
    ElementTheme GetTheme();
    string GetThemeName();
    void SetTheme(ElementTheme theme);
}
