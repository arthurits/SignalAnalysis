namespace $safeprojectname$.Contracts.Services;

public interface IMonospacedFontsService
{
    IReadOnlyList<double> FontSizes { get; }
    IReadOnlyList<$safeprojectname$.Models.FontItem> MonospacedFonts { get; }
}
