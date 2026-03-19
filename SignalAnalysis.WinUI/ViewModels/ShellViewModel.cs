using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Helpers;

using Microsoft.UI.Xaml.Navigation;

namespace SignalAnalysis.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    public partial bool IsBackEnabled {get; set;}
    [ObservableProperty]
    public partial object? Selected {get; set;}

    // Services
    public INavigationService NavigationService { get; }
    public INavigationViewService NavigationViewService { get; }
    private readonly ILocalizationService _localizationService;
    private readonly IMainWindowService _mainWindowService;
    public IMainWindowService MainWindowService => _mainWindowService;

    public IRelayCommand GoBackCommand { get; }

    [ObservableProperty]
    public partial string StrAppDisplayName_Base { get; private set; } = string.Empty;
    [ObservableProperty]
    public partial string StrAppDisplayName_File { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string StrAboutItem { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrAboutToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrLibertyItem { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrLibertyToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrLiftingItem { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrLiftingToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrCarryingItem { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrCarryingToolTip { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrSettingsItem { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string StrSettingsToolTip { get; set; } = string.Empty;


    public ShellViewModel(INavigationService navigationService,
        INavigationViewService navigationViewService,
        ILocalizationService localizationService,
        IMainWindowService mainWindowService)
    {
        // Retrieve the navigation service and navigation view service
        NavigationService               = navigationService;
        NavigationService.Navigated     += OnNavigated;
        NavigationViewService           = navigationViewService;

        GoBackCommand = new RelayCommand(() => NavigationService.GoBack(), () => NavigationService.CanGoBack);

        // Subscribe to localization service events
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Get the MainWindow service
        _mainWindowService = mainWindowService;

        // Set the title union character
        _mainWindowService.TitleUnion = "StrTitleUnion".GetLocalized("Shell");

    }

    public void Dispose()
    {
        NavigationService.Navigated             -= OnNavigated;
        _localizationService.LanguageChanged    -= OnLanguageChanged;
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        // Update the display name and tooltips based on the current language
        StrAppDisplayName_Base = "StrAppDisplayName".GetLocalized("Shell");
        StrAboutItem = "StrAboutItem".GetLocalized("Shell");
        StrAboutToolTip = "StrAboutToolTip".GetLocalized("Shell");
        StrLibertyItem = "StrLibertyItem".GetLocalized("Shell");
        StrLibertyToolTip = "StrLibertyToolTip".GetLocalized("Shell");
        StrLiftingItem = "StrLiftingItem".GetLocalized("Shell");
        StrLiftingToolTip = "StrLiftingToolTip".GetLocalized("Shell");
        StrCarryingItem = "StrCarryingItem".GetLocalized("Shell");
        StrCarryingToolTip = "StrCarryingToolTip".GetLocalized("Shell");
        StrSettingsItem = "StrSettingsItem".GetLocalized("Shell");
        StrSettingsToolTip = "StrSettingsToolTip".GetLocalized("Shell");

    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;
        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem is not null)
        {
            Selected = selectedItem;
        }

        // Notify that the CanExecute state of the GoBackCommand may have changed
        GoBackCommand.NotifyCanExecuteChanged();
    }

    public bool TryGoBack() => NavigationService.GoBack();
}
