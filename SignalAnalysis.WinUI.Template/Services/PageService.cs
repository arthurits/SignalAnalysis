using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using SignalAnalysis.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

namespace SignalAnalysis.Template.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = [];

    public PageService()
    {
        //Configure<LibertyViewModel, LibertyPage>();
        //Configure<SettingsViewModel, SettingsPage>();
        //Configure<AboutViewModel, AboutPage>();
        //Configure<LiftingViewModel, LiftingPage>();

        // Auto-configuración por convención en el ensamblado actual
        AutoConfigureAssembly(Assembly.GetExecutingAssembly());
        // Si quieres escanear más ensamblados, añade llamadas adicionales:
        // AutoConfigureAssembly(typeof(SomeTypeInOtherAssembly).Assembly);
    }

    /// <summary>
    /// Retrieves the Page type associated with the given key (ViewModel full name).
    /// </summary>
    /// <param name="key">ViewModel full name</param>
    /// <returns>Page type</returns>
    /// <exception cref="ArgumentException">Thrown when the key is not found in the collection</exception>
    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    /// <summary>
    /// Exposes the registered ViewModel types (useful for later initialization)
    /// </summary>
    /// <returns>Registered ViewModel types</returns>
    public IEnumerable<Type> GetRegisteredViewModelTypes()
    {
        lock (_pages)
        {
            return _pages.Keys.Select(k => Type.GetType(k)!).Where(t => t != null).ToList();
        }
    }

    /// <summary>
    /// Auto-configure Pages and ViewModels by convention from the given assembly using reflection.
    /// </summary>
    /// <param name="asm">Assemble containing Pages and ViewModels</param>
    private void AutoConfigureAssembly(Assembly asm)
    {
        var pageTypes = asm.GetTypesSafe().Where(t => typeof(Page).IsAssignableFrom(t) && !t.IsAbstract);
        foreach (var pageType in pageTypes)
        {
            // Convención:FooPage -> FooViewModel
            var vmName = pageType.Name.EndsWith("Page")
                ? pageType.Name.Substring(0, pageType.Name.Length - "Page".Length) + "ViewModel"
                : pageType.Name + "ViewModel";

            // Buscar tipo de VM en el mismo assembly primero
            var vmType = asm.GetTypesSafe().FirstOrDefault(t => t.Name == vmName) ?? AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypesSafe())
                    .FirstOrDefault(t => t.Name == vmName);

            if (vmType == null)
            {
                // Opción: registrar un log o ignorar silenciosamente.
                // Aquí optamos por ignorar para no romper el arranque si hay páginas sin VM emparejada.
                continue;
            }

            //// Llamar al Configure genérico usando reflexión para reutilizar validaciones
            //var configureMethod = typeof(PageService).GetMethod("Configure", BindingFlags.NonPublic | BindingFlags.Instance)!
            //    .MakeGenericMethod(vmType, pageType); configureMethod.Invoke(this, null);

            // Call configuration method and make checks
            if (vmType is not null)
            {

                Configure(vmType, pageType);
            }
        }
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }

    private void Configure(Type vmType, Type pageType)
    {
        ArgumentNullException.ThrowIfNull(vmType);

        ArgumentNullException.ThrowIfNull(pageType);

        // Validaciones equivalentes a las de la versión genérica
        if (!typeof(ObservableObject).IsAssignableFrom(vmType))
        {
            throw new ArgumentException($"ViewModel type must inherit ObservableObject: {vmType.FullName}");
        }

        if (!typeof(Page).IsAssignableFrom(pageType))
        {
            throw new ArgumentException($"Page type must inherit Page: {pageType.FullName}");
        }

        var key = vmType.FullName!;
        lock (_pages)
        {
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }
            if (_pages.ContainsValue(pageType))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");
            }

            _pages.Add(key, pageType);
        }
    }
}

// Helper function to avoid ReflectionTypeLoadException
public static class AssemblyExtensions
{
    public static IEnumerable<Type> GetTypesSafe(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t != null)!;
        }
    }
}
