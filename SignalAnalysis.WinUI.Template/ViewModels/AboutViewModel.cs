using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Helpers;
using Windows.System;

namespace SignalAnalysis.Template.ViewModels;

public partial class AboutViewModel : ObservableRecipient
{
    private readonly ILocalizationService _localizationService;

    private readonly string _productName;
    private readonly string _versionNumber;
    private readonly string _copyright;
    private readonly string _companyName;
    private readonly string _companyUrl;

    [ObservableProperty]
    public partial string StrProductName { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string StrVersionNumber { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string StrCopyright { get; private set; } = string.Empty;

    [ObservableProperty]
    public partial string StrDevelopedBy { get; private set; } = string.Empty;

    public string StrCompanyShortName => _companyName;

    public string StrCompanyUrl => _companyUrl; 

    [ObservableProperty]
    public partial string StrLegalText { get; private set; } = string.Empty;

    public AboutViewModel(ILocalizationService localizationService)
    {
        // Subscribe to localization service events
        _localizationService = localizationService;
        _localizationService.LanguageChanged += OnLanguageChanged;

        // Initialize product information from AboutProperties
        _productName = AboutProperties.GetProductName();
        _versionNumber = AboutProperties.GetVersionDescription();
        _copyright = AboutProperties.GetCopyright();
        _companyName = AboutProperties.GetCompanyName();
        _companyUrl = "StrCompanyUrl".GetLocalized("About");

        // Load string resources into binding variables for the UI
        OnLanguageChanged(null, EventArgs.Empty);
    }

    [RelayCommand]
    private async Task OpenCompanyUrlAsync()
    {
        if (Uri.TryCreate(_companyUrl, UriKind.Absolute, out var uri))
        {
            await Launcher.LaunchUriAsync(uri);
        }
    }

    private void OnLanguageChanged(object? sender, EventArgs e)
    {
        StrProductName = string.Format("StrProductName".GetLocalized("About"), _productName);
        StrVersionNumber = string.Format("StrVersionNumber".GetLocalized("About"), _versionNumber);
        StrCopyright = string.Format("StrCopyright".GetLocalized("About"), _copyright);
        //StrDevelopedBy = string.Format("StrDevelopedBy".GetLocalized("About"), _companyName);
        StrDevelopedBy = "StrDevelopedBy".GetLocalized("About");
        StrDevelopedBy = StrDevelopedBy[..^4];
        StrLegalText = string.Format("StrLegalText".GetLocalized("About"), StrCopyright, _companyName);
    }
}
