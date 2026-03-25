using SignalAnalysis.Template.Contracts.Services;
using Windows.ApplicationModel.Resources;

namespace SignalAnalysis.Template.Helpers;

public static class ResourceExtensions
{
    //private static Lazy<ResourceLoader> _resourceLoader = new(() => new ResourceLoader());
    //private static Lazy<ILocalizationService> _localizationService = new(App.GetService<ILocalizationService>);

    public static string GetLocalized(this string resourceKey)
    {
        ////var resourceLoader = new ResourceLoader();
        //var resourceLoader = ResourceLoader.GetForViewIndependentUse();
        //return resourceLoader.GetString(resourceKey);

        var localizationService = App.GetService<ILocalizationService>();
        return localizationService.GetString(resourceKey);
    }

    public static string GetLocalized(this string resourceKey, string resourceMap)
    {
        var localizationService = App.GetService<ILocalizationService>();
        return localizationService.GetString(resourceKey, resourceMap);
    }

    //public static void Refresh()
    //{
    //    // Reset WinRT resource context
    //    Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Reset();

    //    // Substituting the Lazy instances to force a new instance
    //    _resourceLoader = new(() => new ResourceLoader());
    //    _localizationService = new(App.GetService<ILocalizationService>);
    //}
}