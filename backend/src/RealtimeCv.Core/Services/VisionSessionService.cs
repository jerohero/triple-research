using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.Core.Services;

/// <summary>
/// 
/// </summary>
public class VisionSessionService : IVisionSessionService, IDisposable
{
    private ISessionRepository _sessionRepository;
    
    public VisionSessionService(
        ISessionRepository sessionRepository
    )
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Session> Start(int sessionId)
    {
        var spec = new SessionWithVisionSetSpec(sessionId);
        var session = await _sessionRepository.SingleOrDefaultAsync(spec, CancellationToken.None);
        
        Guard.Against.Null(session, nameof(session));

        session = await SetIsActive(session, true);

        return session;
    }

    public async Task Stop()
    {
        await Task.CompletedTask;
    }
    
    private async Task<Session> SetIsActive(Session session, bool isActive)
    {
        session.IsActive = true;
        
        await _sessionRepository.UpdateAsync(session);

        return session;
    }
    
    public void Dispose()
    {
    }
}
