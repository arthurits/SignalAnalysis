namespace SignalAnalysis.Template.Contracts.Services;
public interface IIconLoader
{
    /// <summary>
    /// Asynchronously loads an image from the specified file path and returns a handle to the icon (HICON)
    /// ready to be used.
    /// </summary>
    /// <param name="path">The file path of the image to load (png, jpg, etc.).</param>
    /// <param name="size">The size of the icon to load, typically 16, 32, or 48 pixels.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is a handle to the loaded icon as an <see
    /// cref="IntPtr"/>.</returns>
    Task<IntPtr> LoadIconAsync(string path, uint size);

}
