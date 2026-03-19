using CommunityToolkit.Mvvm.ComponentModel;
using SignalAnalysis.Contracts.Services;

namespace ManualHandling.Services;
public partial class MainWindowService : ObservableObject, IMainWindowService
{
    private readonly WinUIEx.WindowEx _window;

    [ObservableProperty]
    public partial int WindowLeft { get; set; }

    [ObservableProperty]
    public partial int WindowTop { get; set; }

    [ObservableProperty]
    public partial int WindowWidth { get; set; }

    [ObservableProperty]
    public partial int WindowHeight { get; set; }

    [ObservableProperty]
    public partial string Title { get; private set; } = string.Empty;
    [ObservableProperty]
    public partial string TitleMain { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string TitleFile { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string TitleUnion { get; set; } = "-";

    public MainWindowService(WinUIEx.WindowEx window)
    {
        _window = window;
        WindowLeft   = window.AppWindow.Position.X;
        WindowTop    = window.AppWindow.Position.Y;
        WindowWidth  = window.AppWindow.Size.Width;
        WindowHeight = window.AppWindow.Size.Height;

        // Subscribe to window position and size change events
        window.SizeChanged     += OnSizeChanged;
        window.PositionChanged += OnPositionChanged;
    }

    private void OnSizeChanged(object? sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs args)
    {
        WindowWidth  = (int)args.Size.Width;
        WindowHeight = (int)args.Size.Height;
    }

    private void OnPositionChanged(object? sender, Windows.Graphics.PointInt32 e)
    {
        WindowLeft = e.X;
        WindowTop  = e.Y;
    }

    partial void OnTitleChanged(string oldValue, string newValue)
    {
        _window.Title = newValue;
    }

    partial void OnTitleMainChanged(string oldValue, string newValue)
    {
        Title = WindowText();
    }

    partial void OnTitleFileChanged(string oldValue, string newValue)
    {
        Title = WindowText();
    }

    partial void OnTitleUnionChanged(string oldValue, string newValue)
    {
        Title = WindowText();
    }

    /// <summary>
    /// Computes the window text based on the main title, file name, and union string.
    /// </summary>
    /// <returns>The window text</returns>
    public string WindowText()
    {
        string strText;
        var strUnion = $" {TitleUnion} ";

        if (TitleFile.Length > 0)
        {
            strText = $"{strUnion}{TitleFile}";
        }
        else
        {
            var index = TitleMain.IndexOf(strUnion) > -1 ? TitleMain.IndexOf(strUnion) : TitleMain.Length;
            strText = TitleMain[index..];
        }

        return TitleMain + strText;
    }
}
