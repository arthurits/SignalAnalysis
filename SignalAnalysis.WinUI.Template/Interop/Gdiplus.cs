using System.Runtime.InteropServices;

namespace SignalAnalysis.Interop;
internal static partial class Win32
{
    [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
    public static extern GpStatus GdipCreateBitmapFromFile(string filename, out IntPtr bitmap);

    [DllImport("gdiplus.dll")]
    public static extern GpStatus GdipCreateBitmapFromScan0(
        int width,
        int height,
        int stride,
        PixelFormat format,
        IntPtr scan0,
        out IntPtr bitmap
    );

    [DllImport("gdiplus.dll", EntryPoint = "GdipCreateFromImage", CallingConvention = CallingConvention.StdCall, ExactSpelling = true)]
    public static extern GpStatus GdipCreateFromImage(IntPtr image, out IntPtr graphics);

    [DllImport("gdiplus.dll")]
    public static extern GpStatus GdipCreateGraphics(out IntPtr graphics, IntPtr bitmap);

    [DllImport("gdiplus.dll")]
    public static extern GpStatus GdipDeleteGraphics(IntPtr graphics );

    [DllImport("gdiplus.dll")]
    public static extern GpStatus GdipCreateHBITMAPFromBitmap(IntPtr bitmap, out IntPtr hbmReturn, int background);

    [DllImport("gdiplus.dll", ExactSpelling = true)]
    public static extern GpStatus GdipCreateHICONFromBitmap(IntPtr bitmap, out IntPtr hicon);
    
    [DllImport("gdiplus.dll", ExactSpelling = true)]
    public static extern GpStatus GdipDisposeImage(IntPtr image);

    [DllImport("gdiplus.dll")]
    public static extern GpStatus GdipDrawImageRectI(
        IntPtr graphics,
        IntPtr image,
        int x,
        int y,
        int width,
        int height
    );

    [DllImport("gdiplus.dll")]
    public static extern GpStatus GdipGetImageGraphicsContext(IntPtr image, out IntPtr graphics);

    [DllImport("gdiplus.dll")]
    public static extern GpStatus GdipSetInterpolationMode(IntPtr graphics, InterpolationMode mode);

    [DllImport("gdiplus.dll", ExactSpelling = true)]
    public static extern void GdiplusShutdown(IntPtr token);

    [DllImport("gdiplus.dll", ExactSpelling = true)]
    public static extern GpStatus GdiplusStartup(out IntPtr token, ref GdiplusStartupInput input, IntPtr output);
}
