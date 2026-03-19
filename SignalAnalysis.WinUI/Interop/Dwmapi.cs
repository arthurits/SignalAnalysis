using System.Runtime.InteropServices;
using WinRT.Interop;

namespace ManualHandling.Interop;
internal static partial class Win32
{
    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmEnableBlurBehindWindow(
        IntPtr hwnd,
        ref DWM_BLURBEHIND pBlurBehind);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmExtendFrameIntoClientArea(
        IntPtr hwnd,
        ref MARGINS pMarInset);

    [DllImport("dwmapi.dll", PreserveSig = true)]
    public static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        DWMWINDOWATTRIBUTE attribute,
        ref uint pvAttribute,
        int cbAttribute);
}

//private void InitializeWindowEffects()
//    {
//        IntPtr hwnd = WindowNative.GetWindowHandle(this);

//        // 1. Esquinas redondeadas (Windows 11)
//        uint cornerPref = (uint)DWM_WINDOW_CORNER_PREFERENCE.ROUND;
//        DwmApi.DwmSetWindowAttribute(
//            hwnd,
//            DWMWINDOWATTRIBUTE.WINDOW_CORNER_PREFERENCE,
//            ref cornerPref,
//            sizeof(uint));

//        // 2. Modo oscuro inmersivo (Win10 1809+)
//        uint darkMode = 1;
//        DwmApi.DwmSetWindowAttribute(
//            hwnd,
//            DWMWINDOWATTRIBUTE.USE_IMMERSIVE_DARK_MODE,
//            ref darkMode,
//            sizeof(uint));

//        // 3. Extender frame para Acrylic / Aero
//        var margins = new MARGINS { cxLeftWidth = -1, cxRightWidth = -1, cyTopHeight = -1, cyBottomHeight = -1 };
//        DwmApi.DwmExtendFrameIntoClientArea(hwnd, ref margins);

//        // 4. Blur behind clásico (opcional)
//        var blur = new DWM_BLURBEHIND
//        {
//            dwFlags = DwmBlurBehindFlags.ENABLE,
//            fEnable = true,
//            hRgnBlur = IntPtr.Zero,
//            fTransitionOnMaximized = false
//        };
//        DwmApi.DwmEnableBlurBehindWindow(hwnd, ref blur);
//    }
