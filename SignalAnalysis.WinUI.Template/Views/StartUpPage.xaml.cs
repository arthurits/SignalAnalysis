using Microsoft.UI.Xaml.Controls;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.ViewModels;

namespace SignalAnalysis.Views;

/// <summary>
/// StartUpPage to be used as a template.
/// </summary>
public sealed partial class StartUpPage : Page, IDisposable
{
    public StartUpViewModel ViewModel { get; }
    private readonly ILocalizationService _localizationService;

    public StartUpPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<StartUpViewModel>();
        DataContext = ViewModel;
        _localizationService = App.GetService<ILocalizationService>();
    }

    public void Dispose()
    {
    }
}
