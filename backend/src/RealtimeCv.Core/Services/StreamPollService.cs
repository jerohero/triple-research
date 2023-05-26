using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models;

namespace RealtimeCv.Core.Services;

/// <summary>
/// Service that oversees the process of consuming the input, sending it to the inference API and publishing the results.
/// </summary>
public class StreamPollService : IStreamPollService, IDisposable
{
    private readonly IStreamReceiver _streamReceiver;
    private readonly IQueue _queue;

    public StreamPollService(
        IStreamReceiver streamReceiver,
        IQueue queue)
    {
        _streamReceiver = streamReceiver;
        _queue = queue;
    }
    
    public List<string> DetectActiveStreams(List<string> sources)
    {
        Guard.Against.Null(sources, nameof(sources));
        
        return (
            from source in sources
            let isActive = _streamReceiver.CheckConnection(source)
            where isActive 
            select source
        ).ToList();
    }

    public async Task SendStreamPollChunkToQueue(List<string> sources)
    {
        var message = new StreamPollChunkMessage { Sources = sources };
        
        await _queue.SendMessage("stream-poll-chunk", message);
    }

    public void Dispose()
    {
        _streamReceiver.Dispose();
    }
}
