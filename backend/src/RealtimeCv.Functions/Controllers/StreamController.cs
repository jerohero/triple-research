using System;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models;

namespace RealtimeCv.Functions.Controllers;

/// <summary> 
/// </summary>
public class StreamController : BaseController
{
    private readonly ILoggerAdapter<StreamController> _logger;
    private readonly IStreamDetectionService _streamDetectionService;

    public StreamController(
        ILoggerAdapter<StreamController> logger,
        IStreamDetectionService streamDetectionService
    )
    {
        _logger = logger;
        _streamDetectionService = streamDetectionService;
    }

    [Function("PollStreams")]
    public async Task PollStreams(
      [TimerTrigger("*/20 * * * * *")] TimerInfo timerInfo, FunctionContext context)
    {
        await _streamDetectionService.StartPollStreams();
    }

    [Function("StartSessionsForActiveStreams")]
    public void StartSessionsForActiveStreams([QueueTrigger("stream-poll-chunk")] StreamPollChunkMessage message)
    {
        var now = DateTime.UtcNow;

        var activeStreams = _streamDetectionService.StartSessionsForActiveStreams(message);

        var duration = DateTime.UtcNow - now;
        
        _logger.LogInformation(">>> Took " + duration.TotalSeconds + "s to poll " + message.Sources.Count + " streams. ");
        _logger.LogInformation($">>> {activeStreams.Count}");
    }
}
