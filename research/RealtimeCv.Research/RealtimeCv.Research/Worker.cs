using System.Drawing;
using FFMpegCore;
using FFMpegCore.Enums;
using FFMpegCore.Pipes;

namespace RealtimeCv.Research;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly StreamReceiver _streamReceiver;
    private readonly StreamSender _streamSender;

    public Worker(
        ILogger<Worker> logger,
        StreamReceiver streamReceiver,
        StreamSender streamSender
        )
    {
        _logger = logger;
        _streamReceiver = streamReceiver;
        _streamSender = streamSender;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        /* RECEIVING STREAMS */
        // _streamReceiver.ReadStreamFfmpeg();
        // _streamReceiver.ReadStreamOpenCv();
        
        /* SENDING STREAMS */
        // _streamSender.PrepareTarget();
        _streamSender.SendStreamToEndpoint();
    }
}