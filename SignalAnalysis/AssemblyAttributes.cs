using System.Reflection;

namespace ErgoCalc;
public static class AssemblyAttributes
{
    public static string AssemblyTitle
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute.Title != string.Empty)
                {
                    return titleAttribute.Title;
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        }
    }

    public static string AssemblyVersion
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "?";
        }
    }

    public static string AssemblyDescription
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length == 0)
            {
                return string.Empty;
            }
            return String.Concat(((AssemblyDescriptionAttribute)attributes[0]).Description,
                ".",
                System.Environment.NewLine,
                System.Environment.NewLine,
                "No commercial use allowed whatsoever. Contact the author for any inquires.",
                System.Environment.NewLine,
                System.Environment.NewLine,
                "If you find this software useful, please consider supporting it!");
        }
    }

    public static string AssemblyProduct
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0)
            {
                return string.Empty;
            }
            return ((AssemblyProductAttribute)attributes[0]).Product;
        }
    }

    public static string AssemblyCopyright
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length == 0)
            {
                return string.Empty;
            }
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
    }

    public static string AssemblyCompany
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length == 0)
            {
                return string.Empty;
            }
            return ((AssemblyCompanyAttribute)attributes[0]).Company;
        }
    }

}
