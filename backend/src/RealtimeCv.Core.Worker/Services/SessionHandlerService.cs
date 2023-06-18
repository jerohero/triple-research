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
    private readonly IServiceLocator _serviceScopeFactoryLocator;
    private readonly IHttpService _httpService;

    public SessionHandlerHandlerService(
        ILoggerAdapter<SessionHandlerHandlerService> logger,
        IServiceLocator serviceScopeFactoryLocator,
        IHttpService httpService
    )
    {
        _logger = logger;
        _serviceScopeFactoryLocator = serviceScopeFactoryLocator;
        _httpService = httpService;
    }

    public async Task<Session> SetSessionActive(int sessionId)
    {
        using var scope = _serviceScopeFactoryLocator;
        var repository = scope.Get<ISessionRepository>();
        
        var spec = new SessionWithVisionSetSpec(sessionId);
        var session = await repository.SingleOrDefaultAsync(spec, CancellationToken.None);
        
        Guard.Against.Null(session, nameof(session));
        
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
        await _httpService.Post($"{Environment.GetEnvironmentVariable("ApiUrl")}/session/{sessionId}/stop");
    }

    public void Dispose()
    {
    }
}
