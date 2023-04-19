using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using AutoMapper;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
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

    public SessionService(
        ILoggerAdapter<SessionService> logger,
        IMapper mapper,
        ISessionRepository sessionRepository,
        IVisionSetRepository visionSetRepository
    )
    {
        _mapper = mapper;
        _logger = logger;
        _sessionRepository = sessionRepository;
        _visionSetRepository = visionSetRepository;
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
        var sessions = await _sessionRepository.ListAsync();

        return new Result<List<SessionDto>>(_mapper.Map<List<SessionDto>>(sessions));
    }
    
    public async Task<Result<SessionDto>> CreateSession(SessionCreateDto? createDto)
    {
        var validationResult = await new SessionCreateDtoValidator().ValidateAsync(createDto!);

        if (createDto is null || validationResult.Errors.Any())
        {
            return Result<SessionDto>.Invalid(validationResult.AsErrors());
        }

        var visionSet = await _visionSetRepository.GetByIdAsync(createDto.VisionSetId);

        if (visionSet is null)
        {
            return Result<SessionDto>.NotFound();
        }
        
        var mappedSession = _mapper.Map<Session>(createDto);

        var session = await _sessionRepository.AddAsync(mappedSession);

        return new Result<SessionDto>(_mapper.Map<SessionDto>(session));
    }

    // public async Task<Result<SessionDto>> UpdateSession(SessionDto? updateDto)
    // {
    //     var validationResult = await new SessionDtoValidator().ValidateAsync(updateDto!);
    //
    //     if (updateDto is null || validationResult.Errors.Any())
    //     {
    //         return Result<SessionDto>.Invalid(validationResult.AsErrors());
    //     }
    //
    //     var session = await _sessionRepository.GetByIdAsync(updateDto.Id);
    //
    //     if (session is null)
    //     {
    //         return Result<SessionDto>.NotFound();
    //     }
    //
    //     session.UpdateName(updateDto.Name);
    //
    //     await _sessionRepository.UpdateAsync(session);
    //
    //     return new Result<SessionDto>(updateDto);
    // }

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
}
