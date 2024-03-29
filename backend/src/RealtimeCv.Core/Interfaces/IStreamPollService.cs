﻿using System.Collections.Generic;
using System.Threading.Tasks;
using RealtimeCv.Core.Models;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamPollService
{
    Task<List<string>> StartSessionsForActiveStreams(StreamPollChunkMessage message);

    Task StartPollStreams();
    
    void Dispose();
}
