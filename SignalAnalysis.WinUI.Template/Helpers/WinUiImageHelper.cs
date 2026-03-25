using System.Runtime.InteropServices;
using SignalAnalysis.Interop;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace SignalAnalysis.Template.Helpers;

public static class WinUiImageHelper
{
    /// <summary>
    /// Asynchronously decodes an image file (PNG, JPG, BMP, or SVG) into a scaled <c>HBITMAP</c> handle.
    /// </summary>
    /// <remarks>The method supports decoding images in PNG, JPG, BMP, and SVG formats. If both <paramref
    /// name="width"/> and <paramref name="height"/> are greater than 0, the image will be scaled to the specified
    /// dimensions using linear interpolation. The resulting bitmap is in the BGRA8 format with premultiplied
    /// alpha.</remarks>
    /// <param name="path">The file path of the image to decode. Must be a valid path to an existing image file.</param>
    /// <param name="width">The desired width of the resulting bitmap. If set to a value greater than 0, the image will be scaled to this
    /// width. Otherwise, the original image width is used.</param>
    /// <param name="height">The desired height of the resulting bitmap. If set to a value greater than 0, the image will be scaled to this
    /// height. Otherwise, the original image height is used.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a handle to the decoded <c>HBITMAP</c>.</returns>
    /// <example>var hBmp = await WinUiImageHelper.CreateBitmapFromFileAsync(@"C:\icons\miIcono.svg", 32, 32);
    /// DeleteObject(hBmp);</example>
    public static async Task<IntPtr> CreateBitmapFromFileAsync(string path, int width, int height)
    {
        // Open file as a StorageFile
        var file = await StorageFile.GetFileFromPathAsync(path);
        using var stream = await file.OpenAsync(FileAccessMode.Read);

        // Create the proper BitmapDecoder
        var decoder = await BitmapDecoder.CreateAsync(stream);

        // Scale the image to the specified dimensions if needed
        var transform = new BitmapTransform();
        if (width > 0 && height > 0)
        {
            transform.ScaledWidth  = (uint)width;
            transform.ScaledHeight = (uint)height;
            transform.InterpolationMode = BitmapInterpolationMode.Linear;
        }

        // Get the pixels inf BGRA8 format premultiplied
        var pixelData = await decoder.GetPixelDataAsync(
            BitmapPixelFormat.Bgra8,
            BitmapAlphaMode.Premultiplied,
            transform,
            ExifOrientationMode.IgnoreExifOrientation,
            ColorManagementMode.DoNotColorManage);

        var pixels = pixelData.DetachPixelData(); // byte[]

        // Create the DIBSection and copy the data bits
        return CopyToDIBSection(pixels, width, height);
    }

    public static IntPtr CreateBitmapFromFile(string path, int width, int height) => CreateBitmapFromFileAsync(path, width, height).Result;

    /// <summary>
    /// Creates a device-independent bitmap (DIB) section from the specified pixel buffer.
    /// </summary>
    /// <remarks>The created DIB section uses a top-down row order (negative height) and a 32bpp BGRA pixel
    /// format. The pixel data from <paramref name="buffer"/> is copied into the DIB section's memory.</remarks>
    /// <param name="buffer">A byte array containing the pixel data in 32bpp BGRA format. The length of the buffer must be  at least
    /// <paramref name="width"/> * <paramref name="height"/> * 4 bytes.</param>
    /// <param name="width">The width of the bitmap, in pixels.</param>
    /// <param name="height">The height of the bitmap, in pixels.</param>
    /// <returns>A handle to the created DIB section. The caller is responsible for managing the lifetime of the  returned
    /// handle, including releasing it when no longer needed.</returns>
    private static IntPtr CopyToDIBSection(byte[] buffer, int width, int height)
    {
        int stride = width * 4;
        int bufSize = stride * height;

        // Prepares BITMAPINFO (top-down, 32bpp BGRA)
        var bmi = new Win32.BITMAPINFO
        {
            bmiHeader = new Win32.BITMAPINFOHEADER
            {
                biSize        = (uint)Marshal.SizeOf<Win32.BITMAPINFOHEADER>(),
                biWidth       = width,
                biHeight      = -height,
                biPlanes      = 1,
                biBitCount    = 32,
                biCompression = Win32.BI_RGB
            }
        };

        // Create DIBSection
        IntPtr bitsPtr;
        var hBitmap = Win32.CreateDIBSection(
            IntPtr.Zero, ref bmi, Win32.DIB_RGB_COLORS, out bitsPtr, IntPtr.Zero, 0);

        // Copy pixel data to the DIB section
        Marshal.Copy(buffer, 0, bitsPtr, bufSize);
        return hBitmap;
    }

}