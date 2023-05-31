﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Result;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Models.Dto;

namespace RealtimeCv.Core.Interfaces;

public interface ISessionService
{
    Task<Result<SessionDto>> GetSessionById(int sessionId);
    Task<Result<List<SessionDto>>> GetSessionsByVisionSet(int visionSetId);
    Task<Result<SessionDto>> StartSession(SessionStartDto createDto);
    Task<Result<SessionDto>> UpdateSession(SessionDto updateDto);
    Task<Result> DeleteSession(int sessionId);
    Task<Result> StopSession(int sessionId);
    Task<Result<SessionNegotiateDto>> NegotiateSession(int sessionId);
    Task<Result<List<Session>>> GetActiveVisionSetSessionsBySource(int visionSetId, string source);
}