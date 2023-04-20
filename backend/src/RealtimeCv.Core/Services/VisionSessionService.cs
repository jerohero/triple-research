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
    private IKubernetesService _kubernetesService;
    
    public VisionSessionService(
        ISessionRepository sessionRepository,
        IKubernetesService kubernetesService
    )
    {
        _sessionRepository = sessionRepository;
        _kubernetesService = kubernetesService;
    }

    public async Task<Session> Start(int sessionId)
    {
        var spec = new SessionWithVisionSetSpec(sessionId);
        var session = await _sessionRepository.SingleOrDefaultAsync(spec, CancellationToken.None);
        
        Guard.Against.Null(session, nameof(session));

        session.IsActive = true;
        await _sessionRepository.UpdateAsync(session);

        return session;
    }

    public async Task Stop(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        
        Guard.Against.Null(session, nameof(session));
        
        session.IsActive = false;
        await _sessionRepository.UpdateAsync(session);
        
        await _kubernetesService.DeletePod("d");
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
