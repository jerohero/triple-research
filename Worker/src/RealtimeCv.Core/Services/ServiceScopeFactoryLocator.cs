using Microsoft.Extensions.DependencyInjection;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Services;

/// <summary>
/// A wrapper around ServiceScopeFactory to make it easier to fake out with MOQ.
/// </summary>
/// <see cref="https://stackoverflow.com/a/53509491/54288"/>
public sealed class ServiceScopeFactoryLocator : IServiceLocator
{
    private readonly IServiceScopeFactory _factory;
    private IServiceScope _scope;

    public ServiceScopeFactoryLocator(IServiceScopeFactory factory)
    {
        _factory = factory;
    }

    public T Get<T>()
    {
        CreateScope();

        return _scope.ServiceProvider.GetService<T>();
    }

    public IServiceScope CreateScope()
    {
        return _scope ??= _factory.CreateScope();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        _scope = null;
    }
}
