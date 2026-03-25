using System.Reflection;
using Windows.ApplicationModel;

namespace $safeprojectname$.Helpers;

/// <summary>
/// Retireve app values regarding version number, app name, vendor, copyright, etc.
/// </summary>
public static class AboutProperties
{
    // https://stackoverflow.com/questions/70440433/winui-3-runtime-localization
    public static string GetVersionDescription()
    {
        System.Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        //var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView("Localization");
        //Microsoft.Windows.ApplicationModel.Resources.ResourceLoader _resourceLoader = new("Localization");

        //var resourceContext = new Windows.ApplicationModel.Resources.Core.ResourceContext(); // not using ResourceContext.GetForCurrentView
        //var resourceMap = Windows.ApplicationModel.Resources.Core.ResourceManager.Current.MainResourceMap.GetSubtree("Localization/Resources");

        //return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        //return $"{"AppDisplayName"} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";

    }

    public static string GetCopyright()
    {
        string result = string.Empty;

        if (RuntimeHelper.IsMSIX)
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
            {
                result = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        else
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
            {
                result = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }

        }

        return result;
    }

    public static string GetCompanyName()
    {
        string result = string.Empty;

        if (RuntimeHelper.IsMSIX)
        {
            result = Package.Current.PublisherDisplayName;
        }
        else
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length > 0)
            {
                result = ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        return result;
    }

    public static string GetProductName()
    {
        string result = string.Empty;

        if (RuntimeHelper.IsMSIX)
        {
            result = Package.Current.Id.Name;
        }
        else
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length > 0)
                result = ((AssemblyProductAttribute)attributes[0]).Product;

        }

        return result;
    }
}
