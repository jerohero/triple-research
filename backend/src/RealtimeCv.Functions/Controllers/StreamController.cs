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
    private readonly IStreamPollService _streamPollService;

    public StreamController(
        ILoggerAdapter<StreamController> logger,
        IStreamPollService streamPollService
    )
    {
        _logger = logger;
        _streamPollService = streamPollService;
    }

    [Function("PollStreams")]
    public async Task PollStreams(
      [TimerTrigger("*/60 * * * * *")] TimerInfo timerInfo, FunctionContext context)
    {
        await _streamPollService.StartPollStreams();
    }

    [Function("StartSessionsForActiveStreams")]
    public async Task StartSessionsForActiveStreams([QueueTrigger("stream-poll-chunk")] StreamPollChunkMessage message)
    {
        var now = DateTime.UtcNow;

        var activeStreams = await _streamPollService.StartSessionsForActiveStreams(message);

        var duration = DateTime.UtcNow - now;
        
        _logger.LogInformation(">>> Took " + duration.TotalSeconds + "s to poll " + message.Sources.Count + " streams. ");
        _logger.LogInformation($">>> {activeStreams.Count}");
    }
}
