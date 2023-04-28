using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using Newtonsoft.Json;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Specifications;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Models;
using RealtimeCv.Functions.Validators;

namespace RealtimeCv.Functions.Services;

/// <summary>
/// Service for Session related endpoints.
/// </summary>
public class SessionService : ISessionService
{
    private readonly IMapper _mapper;
    private readonly ILoggerAdapter<SessionService> _logger;
    private readonly ISessionRepository _sessionRepository;
    private readonly IVisionSetRepository _visionSetRepository;
    private readonly IKubernetesService _kubernetesService;

    public SessionService(
        ILoggerAdapter<SessionService> logger,
        IMapper mapper,
        ISessionRepository sessionRepository,
        IVisionSetRepository visionSetRepository,
        IKubernetesService kubernetesService
    )
    {
        _mapper = mapper;
        _logger = logger;
        _sessionRepository = sessionRepository;
        _visionSetRepository = visionSetRepository;
        _kubernetesService = kubernetesService;
    }

    public async Task<Result<SessionDto>> GetSessionById(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);

        return session is null
          ? Result<SessionDto>.NotFound()
          : new Result<SessionDto>(_mapper.Map<SessionDto>(session));
    }

    public async Task<Result<List<SessionDto>>> GetSessionsByVisionSet(int visionSetId)
    {
        var sessions = await _sessionRepository.ListAsync(); // TODO: By project

        return new Result<List<SessionDto>>(_mapper.Map<List<SessionDto>>(sessions));
    }
    
    public async Task<Result<SessionDto>> CreateSession(SessionCreateDto? createDto)
    {
        var validationResult = await new SessionCreateDtoValidator().ValidateAsync(createDto!);

        if (createDto is null || validationResult.Errors.Any())
        {
            return Result<SessionDto>.Invalid(validationResult.AsErrors());
        }
        
        var session = _mapper.Map<Session>(createDto);
        
        session.CreatedAt = DateTime.Now;

        var createdSession = await _sessionRepository.AddAsync(session);

        var spec = new SessionWithVisionSetSpec(createdSession.Id);
        session = await _sessionRepository.SingleOrDefaultAsync(spec, CancellationToken.None);
        
        Guard.Against.Null(session, nameof(session));

        createdSession.Pod = $"cv-{createdSession.VisionSet.Name}-{session.Id}";

        await _sessionRepository.UpdateAsync(createdSession);

        return new Result<SessionDto>(_mapper.Map<SessionDto>(session));
    }

    public async Task<Result<SessionDto>> UpdateSession(SessionDto? updateDto)
    {
        var validationResult = await new SessionDtoValidator().ValidateAsync(updateDto!);
    
        if (updateDto is null || validationResult.Errors.Any())
        {
            return Result<SessionDto>.Invalid(validationResult.AsErrors());
        }
    
        var session = await _sessionRepository.GetByIdAsync(updateDto.Id);
    
        if (session is null)
        {
            return Result<SessionDto>.NotFound();
        }
    
        session.Pod = updateDto.Pod;
    
        await _sessionRepository.UpdateAsync(session);
    
        return new Result<SessionDto>(updateDto);
    }

    public async Task<Result> DeleteSession(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);

        if (session is null)
        {
            return Result.NotFound();
        }

        await _sessionRepository.DeleteAsync(session);

        return Result.Success();
    }
    
    public async Task<Result> StopSession(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        
        if (session is null)
        {
            return Result.NotFound();
        }
        
        session.IsActive = false;
        session.StoppedAt = DateTime.Now;
        await _sessionRepository.UpdateAsync(session);
        
        await _kubernetesService.DeletePod(session.Pod);
        
        return Result.Success();
    }
}
