using System.Runtime.InteropServices;
using $safeprojectname$.Contracts.Services;
using $safeprojectname$.Interop;
using Microsoft.UI;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace $safeprojectname$.Services;
public partial class Win2DIconLoader : IIconLoader, IDisposable
{
    private readonly CanvasDevice _device;
    private bool _disposed;

    public Win2DIconLoader()
    {
        _device = CanvasDevice.GetSharedDevice();
    }

    /// <summary>
    /// Asynchronously loads an icon from the specified file path and returns a handle to the icon (HICON).
    /// </summary>
    /// <remarks>This method uses Win2D to create a bitmap from the specified file and converts it to an HICON.
    /// Ensure that the file path points to a valid image file supported by GDI+. The returned HICON must be destroyed
    /// using <see cref="Win32.DestroyIcon"/> or equivalent system calls to avoid resource leaks.</remarks>
    /// <param name="path">The file path of the icon to load. The path must point to a valid image file.</param>
    /// <param name="dim">The size of the icon to load, in pixels. Default is 16 pixels.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a handle to the loaded icon (HICON).
    /// The caller is responsible for releasing the HICON using appropriate system calls when it is no longer needed.</returns>
    public async Task<IntPtr> LoadIconAsync(string path, uint dim = 16)
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(Win2DIconLoader));

        // Load the image using Win2D
        using var bitmap = await CanvasBitmap.LoadAsync(_device, path);

        // Sets the CanvasRenderTarget to the desired size
        var size = new Size(dim, dim);
        using var rt = new CanvasRenderTarget(
            _device,
            (float)size.Width,
            (float)size.Height,
            bitmap.Dpi);

        // Render the bitmap onto the CanvasRenderTarget
        using (var ds = rt.CreateDrawingSession())
        {
            ds.Clear(Colors.Transparent);
            ds.DrawImage(bitmap, new Rect(0, 0, size.Width, size.Height));
        }

        // Gets the ARGB pixel data from the CanvasRenderTarget
        var pixels = rt.GetPixelBytes();

        // Create a DIBSection to hold the pixel data
        IntPtr screenDC = Win32.GetDC(IntPtr.Zero);
        var header = new Win32.BITMAPINFOHEADER
        {
            biSize          = (uint)Marshal.SizeOf<Win32.BITMAPINFOHEADER>(),
            biWidth         = (int)size.Width,
            biHeight        = -(int)size.Height, // top-down
            biPlanes        = 1,
            biBitCount      = 32,
            biCompression   = Win32.BI_RGB,
            biSizeImage     = 0,
            biXPelsPerMeter = 0,
            biYPelsPerMeter = 0,
            biClrUsed       = 0,
            biClrImportant  = 0
        };
        var bmi = new Win32.BITMAPINFO { bmiHeader = header };

        IntPtr ppvBits;
        var hBitmap = Win32.CreateDIBSection(
            screenDC,
            ref bmi,
            Win32.DIB_RGB_COLORS,
            out ppvBits,
            IntPtr.Zero,
            0);

        // Copy the pixel data to the native bitmap
        Marshal.Copy(pixels, 0, ppvBits, pixels.Length);

        Win32.ReleaseDC(IntPtr.Zero, screenDC);

        // Packs the pixel data into an ICONINFO structure and creates an HICON
        var iconInfo = new Win32.ICONINFO
        {
            fIcon     = true,
            xHotspot  = 0,
            yHotspot  = 0,
            hbmMask   = IntPtr.Zero,
            hbmColor  = hBitmap
        };
        IntPtr hIcon = Win32.CreateIconIndirect(ref iconInfo);

        // Delete the intermediate HBITMAP object to free resources
        Win32.DeleteObject(hBitmap);

        return hIcon;
    }

    // Dispose pattern completo
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        // Here we would release any managed resources if needed
        _disposed = true;
    }

    ~Win2DIconLoader()
    {
        Dispose(false);
    }
}
