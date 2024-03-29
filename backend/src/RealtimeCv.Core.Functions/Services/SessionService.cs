﻿using System;
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
using RealtimeCv.Core.Functions.Validators;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models.Dto;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.Core.Functions.Services;

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
    private readonly IPubSub _pubSub;

    public SessionService(
        ILoggerAdapter<SessionService> logger,
        IMapper mapper,
        ISessionRepository sessionRepository,
        IVisionSetRepository visionSetRepository,
        IKubernetesService kubernetesService,
        IPubSub pubSub
    )
    {
        _mapper = mapper;
        _logger = logger;
        _sessionRepository = sessionRepository;
        _visionSetRepository = visionSetRepository;
        _kubernetesService = kubernetesService;
        _pubSub = pubSub;
    }

    public async Task<Result<SessionDto>> GetSessionById(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);

        if (session is null)
        {
            return Result<SessionDto>.NotFound();
        }

        var pod = await _kubernetesService.GetSessionPod(session.Pod);

        var sessionDto = _mapper.Map<SessionDto>(session);
        sessionDto.Status = pod?.Status?.Phase ?? "Terminated";

        return new Result<SessionDto>(sessionDto);
    }

    public async Task<Result<List<SessionDto>>> GetSessionsByVisionSet(int visionSetId)
    {
        var visionSetSpec = new VisionSetWithProjectSpec(visionSetId);
        var visionSet = await _visionSetRepository.SingleOrDefaultAsync(visionSetSpec, CancellationToken.None);
    
        if (visionSet is null)
        {
            return Result<List<SessionDto>>.NotFound();
        }

        Dictionary<string, string>? podToStatus = null;

        try
        {
            var pods = await _kubernetesService.GetVisionSetPods(visionSet.Project.Name, visionSet.Name);
            podToStatus = pods.Items.ToDictionary(pod => pod.Metadata.Name, pod => pod.Status.Phase);
        }
        catch (Exception)
        {
            _logger.LogInformation("Kubernetes connection failed");
        }

        var sessionsSpec = new SessionsByVisionSet(visionSetId);
        var sessions = await _sessionRepository.ListAsync(sessionsSpec, CancellationToken.None);

        var sessionDtos = _mapper.Map<List<SessionDto>>(sessions);

        if (podToStatus is not null)
        {
            foreach (var sessionDto in sessionDtos)
            {
                sessionDto.Status = podToStatus.TryGetValue(sessionDto.Pod, out var status)
                    ? status
                    : "Terminated";
            }            
        }
        else
        {
            foreach (var sessionDto in sessionDtos)
            {
                sessionDto.Status = "ERROR";
            }    
        }


        return new Result<List<SessionDto>>(sessionDtos);
    }
    
    public async Task<Result<SessionDto>> StartSession(SessionStartDto? startDto)
    {
        var result = await CreateSession(startDto);

        if (result.Status != ResultStatus.Ok)
        {
            return result;
        }

        var sessionSpec = new SessionWithVisionSetSpec(result.Value.Id);
        var session = await _sessionRepository.SingleOrDefaultAsync(sessionSpec, CancellationToken.None);

        if (session is null)
        {
            return Result.NotFound("Session not found");
        }
        
        await _kubernetesService.CreateSessionPod(session);
        
        return result;
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
        
        session.StoppedAt = DateTime.Now;
        await _sessionRepository.UpdateAsync(session);
        
        await _kubernetesService.DeletePod(session.Pod);
        
        return Result.Success();
    }
    
    public async Task<Result<SessionNegotiateDto>> NegotiateSession(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        
        if (session is null)
        {
            return Result.NotFound();
        }

        var uri = await _pubSub.Negotiate("predictions", session.Pod);
        
        return new Result<SessionNegotiateDto>(new SessionNegotiateDto(uri.AbsoluteUri));
    }

    public async Task<Result<List<Session>>> GetActiveVisionSetSessionsBySource(VisionSet visionSet, string source)
    {
        var pods = await _kubernetesService.GetVisionSetPods(visionSet.Project.Name, visionSet.Name);

        ActiveVisionSetSessionsBySourceSpec spec = new(visionSet.Id, source, pods.Items);
        var activeSessions = await _sessionRepository.ListAsync(spec, CancellationToken.None);

        return activeSessions;
    }
    
    private async Task<Result<SessionDto>> CreateSession(SessionStartDto? createDto)
    {
        var validationResult = await new SessionStartDtoValidator().ValidateAsync(createDto!);

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
}
