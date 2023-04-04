using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Services;

/// <summary>
/// Service that oversees the process of consuming the input, sending it to the inference API and publishing the results.
/// </summary>
public class StreamPollService : IStreamPollService, IDisposable
{
    private readonly IStreamReceiver _streamReceiver;

    public StreamPollService(
        IStreamReceiver streamReceiver)
    {
        _streamReceiver = streamReceiver;
    }
    
    public List<string> DetectActiveStreams(string[] sources)
    {
        Guard.Against.Null(sources, nameof(sources));
        
        return (
            from source in sources
            let isActive = _streamReceiver.CheckConnection(source)
            where isActive 
            select source
        ).ToList();
    }

    public void Dispose()
    {
        _streamReceiver.Dispose();
    }
}
