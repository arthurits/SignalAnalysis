using CommunityToolkit.Mvvm.ComponentModel;
using $safeprojectname$.Contracts.Services;

namespace $safeprojectname$.ViewModels;

public partial class StartUpViewModel: ObservableRecipient
{
    private readonly ILocalizationService _localizationService;

    public StartUpViewModel(ILocalizationService localizationService)
    {
        // Subscribe to localization service events
        _localizationService = localizationService;
        _localizationService.LanguageChanged -= OnLanguageChanged;
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Load string resources into binding variables for the UI
        OnLanguageChanged(null, EventArgs.Empty);
    }

    public void Dispose()
    {
        _localizationService.LanguageChanged -= OnLanguageChanged;
    }
}
