﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.Result;
using RealtimeCv.Functions.Models;

namespace RealtimeCv.Functions.Interfaces;

public interface ISessionService
{
    Task<Result<SessionDto>> GetSessionById(int sessionId);
    Task<Result<List<SessionDto>>> GetSessionsByVisionSet(int visionSetId);
    Task<Result<SessionDto>> CreateSession(SessionCreateDto? createDto);
    Task<Result<SessionDto>> UpdateSession(SessionDto? updateDto);
    Task<Result> DeleteSession(int sessionId);
}