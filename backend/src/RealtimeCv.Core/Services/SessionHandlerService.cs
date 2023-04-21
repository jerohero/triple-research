using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.Core.Services;

/// <summary>
/// 
/// </summary>
public class SessionHandlerHandlerService : ISessionHandlerService, IDisposable
{
    private readonly ILoggerAdapter<SessionHandlerHandlerService> _logger;
    private IKubernetesService _kubernetesService;
    private IServiceLocator _serviceScopeFactoryLocator;
    
    public SessionHandlerHandlerService(
        ILoggerAdapter<SessionHandlerHandlerService> logger,
        IServiceLocator serviceScopeFactoryLocator,
        IKubernetesService kubernetesService
    )
    {
        _logger = logger;
        _kubernetesService = kubernetesService;
        _serviceScopeFactoryLocator = serviceScopeFactoryLocator;
    }

    public async Task<Session> SetSessionActive(int sessionId)
    {
        // TODO: Probably shouldn't do this here?
        using var scope = _serviceScopeFactoryLocator.CreateScope();
        var repository =
            scope.ServiceProvider
                .GetService<ISessionRepository>();
        
        var spec = new SessionWithVisionSetSpec(sessionId);
        var session = await repository.SingleOrDefaultAsync(spec, CancellationToken.None);
        
        Guard.Against.Null(session, nameof(session));

        session.IsActive = true;
        await repository.UpdateAsync(session);

        return session;
    }

    public async Task EndSession(int sessionId)
    {
        using var scope = _serviceScopeFactoryLocator.CreateScope();
        var repository =
            scope.ServiceProvider
                .GetService<ISessionRepository>();
        
        var session = await repository.GetByIdAsync(sessionId);
        
        Guard.Against.Null(session, nameof(session));
        
        session.IsActive = false;
        await repository.UpdateAsync(session);

        await _kubernetesService.DeletePod(session.Pod);
    }
    
    // private async Task<Session> SetIsActive(Session session, bool isActive)
    // {
    //     session.IsActive = true;
    //     
    //     await _sessionRepository.UpdateAsync(session);
    //
    //     return session;
    // }
    
    public void Dispose()
    {
    }
}
