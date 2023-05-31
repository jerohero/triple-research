using Ardalis.GuardClauses;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.Core.Worker.Services;

/// <summary>
/// 
/// </summary>
public class SessionHandlerHandlerService : ISessionHandlerService, IDisposable
{
    private readonly ILoggerAdapter<SessionHandlerHandlerService> _logger;
    private IKubernetesService _kubernetesService;
    private IServiceLocator _serviceScopeFactoryLocator;
    private IHttpService _httpService;

    public SessionHandlerHandlerService(
        ILoggerAdapter<SessionHandlerHandlerService> logger,
        IServiceLocator serviceScopeFactoryLocator,
        IKubernetesService kubernetesService,
        IHttpService httpService
    )
    {
        _logger = logger;
        _kubernetesService = kubernetesService;
        _serviceScopeFactoryLocator = serviceScopeFactoryLocator;
        _httpService = httpService;
    }

    public async Task<Session> SetSessionActive(int sessionId)
    {
        // TODO: Is this the right approach?
        using var scope = _serviceScopeFactoryLocator;
        var repository = scope.Get<ISessionRepository>();
        
        var spec = new SessionWithVisionSetSpec(sessionId);
        var session = await repository.SingleOrDefaultAsync(spec, CancellationToken.None);
        
        Guard.Against.Null(session, nameof(session));

        session.IsActive = true;
        session.StartedAt = DateTime.Now;
        await repository.UpdateAsync(session);

        return session;
    }
    
    public async Task<TrainedModel> GetSessionTrainedModel(int sessionId)
    {
        using var scope = _serviceScopeFactoryLocator;
        var repository = scope.Get<ITrainedModelRepository>();
        
        var spec = new TrainedModelBySessionSpec(sessionId);
        var session = await repository.SingleOrDefaultAsync(spec, CancellationToken.None);

        Guard.Against.Null(session, nameof(session));

        return session;
    }

    public async Task EndSession(int sessionId)
    {
        _logger.LogInformation("CLOSING SESSION {sessionId}", sessionId);
        await _httpService.Post($"http://localhost:7071/api/session/{sessionId}/stop");
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
