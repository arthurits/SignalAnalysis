using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Interop;

namespace SignalAnalysis.Services;

public partial class GdiPlusIconLoader : IIconLoader, IDisposable
{
    private static readonly Lock _sync = new();
    private IntPtr _gdiplusToken;
    private static int _refCount;

    // Control the lifecycle of the library:
    // _started indicates whether GDI+ has been initialized
    // _disposed indicates whether the object has been disposed
    private bool _started;
    private bool _disposed;

    public GdiPlusIconLoader()
    {
        Start();
    }

    /// <summary>
    /// Initializes the GDI+ library for the current application, ensuring it is ready for use.
    /// </summary>
    /// <remarks>This method is thread-safe and can be called multiple times. The GDI+ library is initialized
    /// only once, regardless of the number of calls, and subsequent calls will increment an internal reference count.
    /// If the library has already been started, this method will return immediately.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the GDI+ library fails to initialize.</exception>
    public void Start()
    {
        lock (_sync)
        {
            if (_started)
            {
                return;
            }

            if (_refCount++ == 0)
            {
                var input = new Win32.GdiplusStartupInput
                {
                    GdiplusVersion           = 1u,
                    SuppressBackgroundThread = false,
                    SuppressExternalCodecs   = false
                };

                var status = Win32.GdiplusStartup(
                    out _gdiplusToken,
                    ref input,
                    IntPtr.Zero);

                if (status != Win32.GpStatus.Ok)
                {
                    _refCount--;
                    throw new InvalidOperationException($"GDI+ startup failed: {status}");
                }
            }

            _started = true;
        }
    }

    /// <summary>
    /// Loads an HICON from the specified file path, scaling it to the desired size, and returns a handle to the icon (HICON).
    /// </summary>
    /// <remarks>This method uses GDI+ to create a bitmap from the specified file and converts it to an HICON.
    /// Ensure that the file path points to a valid image file supported by GDI+. The returned HICON must be destroyed
    /// using <see cref="Win32.DestroyIcon"/> or equivalent system calls to avoid resource leaks.</remarks>
    /// <param name="path">The file path of the icon to load. The path must point to a valid image file.</param>
    /// <param name="size">The size of the icon to load, in pixels. Default is 16 pixels.</param>
    /// <returns>A handle to the loaded icon (HICON). The caller is responsible for releasing the HICON
    /// using appropriate system calls (<see cref="Win32.DestroyIcon"/>) when it is no longer needed.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operation to create the HBITMAP fails.</exception>
    public IntPtr LoadIcon(string path, uint size = 16)
    {
        // Check if the object has been disposed
        ObjectDisposedException.ThrowIf(_disposed, nameof(GdiPlusIconLoader));

        // Create a Gdi+ bitmap from the file path and scale it to the specified size
        var scaledGdiBmp = CreateScaledBitmap(path, (int)size, (int)size);

        try
        {
            // Convert the GDI+ bitmap to HBITMAP
            var status = Win32.GdipCreateHICONFromBitmap(scaledGdiBmp, out var hIcon);
            if (status != Win32.GpStatus.Ok)
            {
                throw new InvalidOperationException($"GdipCreateHICONFromBitmap failed with code ({status})");
            }

            return hIcon;
        }
        finally
        {
            // Free the GDI+ bitmap
            Win32.GdipDisposeImage(scaledGdiBmp);
        }
    }

    public IntPtr LoadIcon(string path)
    {
        // Check if the object has been disposed
        ObjectDisposedException.ThrowIf(_disposed, nameof(GdiPlusIconLoader));

        // Create a GDI+ bitmap from the file path
        var status = Win32.GdipCreateBitmapFromFile(path, out var gdiBitmap);
        if (status != Win32.GpStatus.Ok || gdiBitmap == 0)
        {
            return IntPtr.Zero;
        }

        // Convert the GDI+ bitmap to an HICON
        status = Win32.GdipCreateHICONFromBitmap(gdiBitmap, out var hIcon);

        // Free the GDI+ bitmap
        _ = Win32.GdipDisposeImage(gdiBitmap);

        if (status != Win32.GpStatus.Ok || hIcon == 0)
        {
            return IntPtr.Zero;
        }

        // Return the HICON. The caller is responsible for destroying the HICON when done.
        return hIcon;
    }

    public Task<IntPtr> LoadIconAsync(string path, uint size = 16) => Task.Run(() => LoadIcon(path, size));

    public Task<IntPtr> LoadIconAsync(string path) => Task.Run(() => LoadIcon(path));

    /// <summary>
    /// Loads an HBITMAP handle from the specified image file, scaling it to the desired size.
    /// </summary>
    /// <remarks>This method uses GDI+ to load and scale the image, and then converts it to an HBITMAP handle.
    /// Ensure that the provided file path is valid and accessible. The method performs cleanup of intermediate
    /// resources but does not manage the lifetime of the returned HBITMAP handle, which should be disposed
    /// by the caller using <see cref="Win32.DeleteObject"/>.</remarks>
    /// <param name="path">The file path of the image to load. The path must point to a valid image file.</param>
    /// <param name="size">The desired width and height, in pixels, to which the image will be scaled. The default value is 16.</param>
    /// <returns>A handle to the HBITMAP representing the scaled image. The caller is responsible for releasing the handle using
    /// appropriate native methods such as <see cref="Win32.DeleteObject"/> to avoid resource leaks.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the operation to create the HBITMAP fails.</exception>
    public IntPtr LoadHBitmap(string path, uint size = 16)
    {
        // Check if the object has been disposed
        ObjectDisposedException.ThrowIf(_disposed, nameof(GdiPlusIconLoader));

        // Create a Gdi+ bitmap from the file path and scale it to the specified size
        var scaledGdiBmp = CreateScaledBitmap(path, (int)size, (int)size);

        try
        {
            // Convert the GDI+ bitmap to HBITMAP
            var status = Win32.GdipCreateHBITMAPFromBitmap(scaledGdiBmp, out var hBitmap, 0);
            if (status != Win32.GpStatus.Ok)
            {
                throw new InvalidOperationException($"GdipCreateHBITMAPFromBitmap failed with code ({status})");
            }

            return hBitmap;
        }
        finally
        {
            // Free the GDI+ bitmap
            Win32.GdipDisposeImage(scaledGdiBmp);
        }
    }

    public IntPtr LoadHBitmap(string path)
    {
        // Load the GDI+ bitmap from the file path
        var status = Win32.GdipCreateBitmapFromFile(path, out var gdiBitmap);
        if (status != Win32.GpStatus.Ok || gdiBitmap == 0)
        {
            return IntPtr.Zero;
        }

        // Convert the GDI+ bitmap to an HBITMAP
        status = Win32.GdipCreateHBITMAPFromBitmap(gdiBitmap, out var hBmp, 0);

        // Free the GDI+ bitmap
        _ = Win32.GdipDisposeImage(gdiBitmap);

        if (status != Win32.GpStatus.Ok)
        {
            return IntPtr.Zero;
        }

        // Return the HBITMAP. The caller is responsible for destroying the HBITMAP when done
        return hBmp;
    }

    public Task<IntPtr> LoadHBitmapAsync(string path, uint size = 16) => Task.Run(() => LoadHBitmap(path, size));

    public Task<IntPtr> LoadHBitmapAsync(string path) => Task.Run(() => LoadHBitmap(path));

    /// <summary>
    /// Creates a scaled bitmap from the specified image file, resizing it to the given dimensions.
    /// </summary>
    /// <remarks>This method uses GDI+ to load the source image, create a new bitmap with the specified
    /// dimensions, and draw the scaled image onto the new bitmap using high-quality bicubic interpolation. The returned
    /// bitmap is in 32bpp ARGB format and must be disposed by the caller.</remarks>
    /// <param name="path">The file path of the source image. The file must exist and be a valid image format supported by GDI+.</param>
    /// <param name="targetWidth">The desired width of the scaled bitmap, in pixels. Must be greater than 0.</param>
    /// <param name="targetHeight">The desired height of the scaled bitmap, in pixels. Must be greater than 0.</param>
    /// <returns>A handle to the scaled bitmap as an <see cref="IntPtr"/>. The caller is responsible for disposing of the bitmap
    /// using appropriate GDI+ cleanup methods.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the source image cannot be loaded, the destination bitmap cannot be created, or an error occurs during
    /// scaling.</exception>
    private IntPtr CreateScaledBitmap(string path, int targetWidth, int targetHeight)
    {
        // Load the GDI+ bitmap from the file path
        var status = Win32.GdipCreateBitmapFromFile(path, out var srcGdiBmp);
        if (status != Win32.GpStatus.Ok || srcGdiBmp == 0)
        {
            throw new InvalidOperationException($"GdipCreateBitmapFromFile failed with code ({status})");
        }

        try
        {
            // Create an empty destination Gdi+ bitmap (ARGB format)
            status = Win32.GdipCreateBitmapFromScan0(
                targetWidth,
                targetHeight,
                targetWidth * 4,                    // stride = width * bytesByPixel
                Win32.PixelFormat.Format32bppARGB,  // 32 bppp ARGB format
                IntPtr.Zero,                        // so that GDI+ reserves the buffer memory
                out var dstGdiBmp);
            if (status != Win32.GpStatus.Ok)
            {
                throw new InvalidOperationException($"GdipCreateBitmapFromScan0 failed with code ({status})");
            }

            try
            {
                // Create a graphics object from the destination bitmap
                status = Win32.GdipGetImageGraphicsContext(dstGdiBmp, out var graphics);
                if (status != Win32.GpStatus.Ok)
                {
                    throw new InvalidOperationException($"GdipCreateFromImage failed with code ({status})");
                }

                try
                {
                    // Modify the graphics object to use high-quality interpolation
                    Win32.GdipSetInterpolationMode(graphics, Win32.InterpolationMode.HighQualityBicubic);

                    // Draw the source bitmap onto the graphics object
                    Win32.GdipDrawImageRectI(graphics, srcGdiBmp, 0, 0, targetWidth, targetHeight);
                }
                finally
                {
                    Win32.GdipDeleteGraphics(graphics);
                }
            }
            catch
            {
                // If there was an error while drawing into destination, clean up the destination bitmap
                Win32.GdipDisposeImage(dstGdiBmp);
                throw;
            }

            return dstGdiBmp;
        }
        finally
        {
            // Always clean up the source bitmap
            Win32.GdipDisposeImage(srcGdiBmp);
        }
    }

    /// <summary>
    /// Shuts down the current instance, releasing any associated resources.
    /// </summary>
    /// <remarks>This method decrements the internal reference count and, if it reaches zero, releases
    /// resources associated with the instance. It is thread-safe and ensures  that shutdown operations are performed
    /// only once when no references remain.</remarks>
    public void Shutdown()
    {
        lock (_sync)
        {
            if (!_started)
            {
                return;
            }

            if (--_refCount == 0 && _gdiplusToken != IntPtr.Zero)
            {
                Win32.GdiplusShutdown(_gdiplusToken);
                _gdiplusToken = IntPtr.Zero;
            }

            _started = false;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        Shutdown();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    // In case the Dispose method is not called, we ensure that GDI+ is shut down when the object is finalized.
    ~GdiPlusIconLoader() => Dispose();
}
