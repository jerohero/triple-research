using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models;
using RealtimeCv.Core.Models.Dto;
using RealtimeCv.Core.Specifications;

namespace RealtimeCv.Core.Functions.Services;

/// <summary>
/// Service that oversees the process of consuming the input, sending it to the inference API and publishing the results.
/// </summary>
public class StreamDetectionService : IStreamDetectionService, IDisposable
{
    private readonly IStreamReceiver _streamReceiver;
    private readonly IVisionSetRepository _visionSetRepository;
    private readonly ISessionService _sessionService;
    private readonly IQueue _queue;
    private const int SourceChunkSize = 10;

    public StreamDetectionService(
        IStreamReceiver streamReceiver,
        IVisionSetRepository visionSetRepository,
        ISessionService sessionService,
        IQueue queue)
    {
        _streamReceiver = streamReceiver;
        _visionSetRepository = visionSetRepository;
        _sessionService = sessionService;
        _queue = queue;
    }
    
    public async Task StartPollStreams()
    {
        var visionSets = await _visionSetRepository.ListAsync();

        foreach (var message in from visionSet in visionSets
                 let chunks = visionSet.Sources.Chunk(SourceChunkSize)
                 from chunk in chunks
                 select new StreamPollChunkMessage
                 {
                     VisionSetId = visionSet.Id,
                     Sources = chunk.ToList()
                 })
        {
            await _queue.SendMessage("stream-poll-chunk", message);
        }
    }
    
    public async Task<List<string>> StartSessionsForActiveStreams(StreamPollChunkMessage message)
    {
        Guard.Against.Null(message, nameof(message));
        
        var activeStreams = (
            from source in message.Sources
            let isActive = _streamReceiver.CheckConnection(source)
            where isActive 
            select source
        ).ToList();

        foreach (var stream in activeStreams)
        {
            var activeSessions = await _sessionService
                .GetActiveVisionSetSessionsBySource(message.VisionSetId, stream);

            if (activeSessions.Value.Count > 0)
            {
                continue;
            }
            
            await Task.Run(() => 
                _sessionService.StartSession(new SessionStartDto(message.VisionSetId, stream))
            );
        }

        return activeStreams;
    }
    
    public void Dispose()
    {
        _streamReceiver.Dispose();
    }
}
