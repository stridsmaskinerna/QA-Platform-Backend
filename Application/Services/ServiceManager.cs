namespace Application.Services;

public class ServiceManager(
    Lazy<IBaseService> baseService
) : IServiceManager
{
    public IBaseService BaseService => baseService.Value;
}
