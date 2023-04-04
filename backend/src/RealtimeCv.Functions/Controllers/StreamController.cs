using System;
using Microsoft.Azure.Functions.Worker;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Functions.Controllers;

/// <summary>
/// 
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
    public void PollStreams(
      [TimerTrigger("*/5 * * * * *")] TimerInfo timerInfo, FunctionContext context)
    {
        var sources = new[] { "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5", "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9fx" };
        
        var activeStreams = _streamPollService.DetectActiveStreams(sources);
        
        _logger.LogInformation("Active streams: {activeStreams}", activeStreams);
    }
}
