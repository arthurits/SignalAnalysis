using System.Globalization;
using System.Resources;

namespace System.Globalization;

/// <summary>
/// Provides some utilities for globalization/localization purposes
/// </summary>
public static class GlobalizationUtilities
{
    /// <summary>
    /// Retrieve all (language/culture) assembly resources in a type
    /// </summary>
    /// <param name="baseName">The root name of the resource file without its extension but including any fully
    ///     qualified namespace name. For example, the root name for the resource file named
    ///     MyApplication.MyResource.en-US.resources is MyApplication.MyResource</param>
    /// <param name="assembly">Specifies the main assembly that contains the resources</param>
    /// <returns>Collection of CultureInfo matches</returns>
    /// <seealso cref="https://stackoverflow.com/questions/553244/programmatic-way-to-get-all-the-available-languages-in-satellite-assemblies"/>
    public static IEnumerable<CultureInfo> GetAvailableCultures(string baseName, Reflection.Assembly assembly)
    {
        List<CultureInfo> result = new();

        ResourceManager rm = new(baseName, assembly);

        CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
        foreach (CultureInfo culture in cultures)
        {
            try
            {
                if (culture.Equals(CultureInfo.InvariantCulture)) continue; //do not use "==", won't work

                ResourceSet? rs = rm.GetResourceSet(culture, true, false);
                if (rs is not null)
                    result.Add(culture);
            }
            catch (CultureNotFoundException)
            {
                //NOP
            }
        }
        return result;
    }
}
