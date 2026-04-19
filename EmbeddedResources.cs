using System.Runtime.CompilerServices;

namespace SignalAnalysis;

/// <summary>
/// Load graphics resources from disk
/// </summary>
public class GraphicsResources
{
	public const string AppLogo = @"images\logo.ico";
	public const string AppLogo256 = @"images\logo@256.png";
	public const string IconExit = @"images\exit.ico";
	public const string IconOpen = @"images\openfolder.ico";
	public const string IconExport = @"images\save.ico";
	public const string IconSettings = @"images\settings.ico";
	public const string IconAbout = @"images\about.ico";

	/// <summary>
	/// Loads a graphics resource from a disk location
	/// </summary>
	/// <typeparam name="T">Type of resource to be loaded</typeparam>
	/// <param name="fileName">File name (absolute or relative to the working directory) to load resource from</param>
	/// <returns>The graphics resource</returns>
	public static T? Load<T> (string fileName)
    {
		T? resource = default;
		try
		{
			if (File.Exists(fileName))
			{
				if (typeof(T).Equals(typeof(System.Drawing.Image)))
					resource = (T)(object)Image.FromFile(fileName);
				else if (typeof(T).Equals(typeof(Icon)))
					resource = (T)(object)new Icon(fileName);
				else if (typeof(T).Equals(typeof(Cursor)))
					resource = (T)(object)new Cursor(fileName);
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(
				$"Unexpected error while loading the {fileName} graphics resource.{Environment.NewLine}{ex.Message}",
				"Loading error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
		return resource;
	}

	/// <summary>
	/// Creates a bitmap from an icon file
	/// </summary>
	/// <param name="fileName">File name (absolute or relative to the working directory) to load resource from</param>
	/// <param name="width">Width in pixels</param>
	/// <param name="height">Height in pixels. If omitted, the width is used instead</param>
	/// <returns>Bitmap from the icon resource</returns>
	public static Bitmap? LoadIcon(string fileName, int width, int? height = null)
	{
		Bitmap? resource = null;
		try
		{
			if (File.Exists(fileName))
			{
				resource = new Icon(fileName, width, height ?? width).ToBitmap();
			}
		}
		catch (Exception ex)
		{
			MessageBox.Show(
				$"Unexpected error while loading the {fileName} icon resource.{Environment.NewLine}{ex.Message}",
				"Loading error",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
		return resource;

	}
}
