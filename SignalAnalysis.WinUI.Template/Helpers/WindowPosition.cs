using Microsoft.UI.Xaml;
using System;

namespace SignalAnalysis.Template.Helpers;

public static class WindowPosition
{
    public static void CenterWindow(IntPtr hWnd)
    {
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        CenterWindow(appWindow);
    }

    public static void CenterWindow(Window window)
    {
        CenterWindow(window.AppWindow);
    }

    public static void CenterWindow(Microsoft.UI.Windowing.AppWindow? appWindow = null)
    {
        if (appWindow is not null)
        {
            Microsoft.UI.Windowing.DisplayArea displayArea = Microsoft.UI.Windowing.DisplayArea.GetFromWindowId(appWindow.Id, Microsoft.UI.Windowing.DisplayAreaFallback.Nearest);
            if (displayArea is not null)
            {
                var CenteredPosition = appWindow.Position;
                CenteredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
                CenteredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
                appWindow.Move(CenteredPosition);
            }
        }
    }

    public static void SetWindowSize(IntPtr hWnd, int width, int height)
    {
        Microsoft.UI.WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
        Microsoft.UI.Windowing.AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
        appWindow?.Resize(new Windows.Graphics.SizeInt32 { Width = width, Height = height });
    }

    public static void SetWindowSize(Window window, int width, int height)
    {
        window.AppWindow?.Resize(new Windows.Graphics.SizeInt32 { Width = width, Height = height });
    }
}