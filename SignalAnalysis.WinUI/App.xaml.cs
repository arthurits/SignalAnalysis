using CommunityToolkit.WinUI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using SignalAnalysis.Activation;
using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Helpers;
using SignalAnalysis.Models;
using SignalAnalysis.Services;
using System.Diagnostics;
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

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.

        // Show error details
        System.Diagnostics.Debug.WriteLine($"UI thread error: {e.Exception}");

        // Mark the exception as handled so that the process does not finish
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object? s, System.UnhandledExceptionEventArgs e)
    {
        Debug.WriteLine($"Non-UI thread error: {(e.ExceptionObject as Exception)?.Message}");
        // There is no e.Handled; so the process will finish after any logging action
    }

    private void TaskScheduler_UnobservedTaskException(object? s, UnobservedTaskExceptionEventArgs e)
    {
        Debug.WriteLine($"Task unobserved: {e.Exception.Message}");
        e.SetObserved();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        // Handle base event implementation
        base.OnLaunched(args);

        // Handle the main window closing events
        MainWindow.AppWindow.Closing += OnClosing;
        MainWindow.Closed += OnClosed;
    }

    private async void OnClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        args.Cancel = true; // https://github.com/microsoft/WindowsAppSDK/issues/3209

        if (await ConfirmAppCloseAsync())
        {
            MainWindow.Close();
            args.Cancel = false; // Allow the app to close
        }
    }

    public static async Task<bool> ConfirmAppCloseAsync()
    {
        var result = await MessageBox.Show(
            "MsgBoxExitContent".GetLocalized("MessageBox"),
            "MsgBoxExitTitle".GetLocalized("MessageBox"),
            primaryButtonText: "MsgBoxExitPrimary".GetLocalized("MessageBox"),
            closeButtonText: "MsgBoxExitCancel".GetLocalized("MessageBox"),
            defaultButton: MessageBox.MessageBoxButtonDefault.CloseButton,
            icon: MessageBox.MessageBoxImage.Question);

        var ClosingConfirmed = result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary;

        if (ClosingConfirmed)
        {
            var settings = App.GetService<ILocalSettingsService<AppSettings>>();
            //await settings.SaveSettingKeyAsync<string>("isTrue","yes");
            if (settings.GetValues.WindowPosition)
            {
                settings.GetValues.WindowLeft = MainWindow.AppWindow.Position.X;
                settings.GetValues.WindowTop = MainWindow.AppWindow.Position.Y;
                settings.GetValues.WindowWidth = MainWindow.AppWindow.Size.Width;
                settings.GetValues.WindowHeight = MainWindow.AppWindow.Size.Height;
            }

            settings.GetValues.AppCultureName = App.GetService<ILocalizationService>().CurrentLanguage;

            // No need to save the theme here, as it is already set in SettingsViewModel OnThemeChanged
            //var themeService = App.GetService<IThemeSelectorService>();
            //settings.GetValues.ThemeName = themeService.GetThemeName();

            await settings.SaveSettingFileAsync();

            // Set the startup enabled state based on the settings
            //var startupService = App.GetService<IStartupService>();
            //startupService.SetStartupEnabled(settings.GetValues.LaunchAtStartup);
        }

        return ClosingConfirmed;
    }

    /// <summary>
    /// This event is fired after the window is closed. It formally ends the app and disposes the host.
    /// It also calls Dispose on all the services.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private async void OnClosed(object sender, WindowEventArgs args)
    {
        App.MainWindow.DispatcherQueue.TryEnqueue(
            DispatcherQueuePriority.Low,
            new DispatcherQueueHandler(async () =>
            {
                await Host.StopAsync();
                Host.Dispose();
            }));
    }
}
