namespace $safeprojectname$.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string key);
    IEnumerable<Type> GetRegisteredViewModelTypes();
}
