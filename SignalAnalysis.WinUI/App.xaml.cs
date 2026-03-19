using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Windows.UI.ApplicationSettings;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SignalAnalysis;
/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    public IHost Host { get; }

    public static T GetService<T>() where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public static UIElement? AppTitlebar { get; set; }


    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddTransient<INavigationViewService, NavigationViewService>();
            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<ILocalizationService, LocalizationService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddSingleton<ILocalSettingsService<AppSettings>, LocalSettingsService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IMonospacedFontsService, MonospacedFontsService>();

            // Tray Icon Service
            services.AddSingleton<GdiPlusIconLoader>();
            services.AddSingleton<Win2DIconLoader>();
            services.AddSingleton<IconLoaderFactory>();
            services.AddSingleton<ITrayIconService, TrayIconService>(sp =>
            {
                var iconFactory = sp.GetRequiredService<IconLoaderFactory>();
                var localSettings = sp.GetRequiredService<ILocalSettingsService<AppSettings>>();
                return new TrayIconService(MainWindow, iconFactory, localSettings);
            });

            // Register the MainWindow service. We use the factory method instantiation of MainWindow but
            // leave the option to use the inline if needed in the future.
            services.AddSingleton<WindowEx>(sp => MainWindow);
            services.AddSingleton<IMainWindowService, MainWindowService>();
            //services.AddSingleton<IMainWindowService>(sp => new MainWindowService(MainWindow));

            // Views and ViewModels
            services.AddSingleton<LiftingViewModel>();
            services.AddSingleton<LiftingPage>();
            services.AddSingleton<AboutViewModel>();
            services.AddTransient<AboutPage>();
            services.AddSingleton<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddSingleton<LibertyViewModel>();
            services.AddSingleton<LibertyPage>();
            services.AddSingleton<CarryingViewModel>();
            services.AddSingleton<CarryingPage>();
            services.AddTransient<ShellPage>();
            services.AddSingleton<ShellViewModel>();

            // Configuration
        }).
        Build();

        UnhandledException += App_UnhandledException;
        // In case we need to catch exceptions outside the UI thread.
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {

    }
}
