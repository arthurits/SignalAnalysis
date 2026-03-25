using SignalAnalysis.Template.Services;
using SignalAnalysis.Template.Contracts.Services;

namespace SignalAnalysis.Template.Services;
public class IconLoaderFactory(GdiPlusIconLoader gdi, Win2DIconLoader win2d) : IIconLoaderFactory
{
    private readonly GdiPlusIconLoader _gdi = gdi;
    private readonly Win2DIconLoader _win2d = win2d;

    public IIconLoader GetLoader(IconLoaderType type)
    {
        IIconLoader value = type switch
        {
            IconLoaderType.GdiPlus => _gdi,
            IconLoaderType.Win2D => _win2d,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
        return value;
    }
}