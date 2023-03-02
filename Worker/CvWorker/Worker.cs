using Emgu.CV;

namespace CvWorker;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private const string Rtmp = "rtmp://";
    private const string Rtsp = "rtsp://";
    private const string Http = "http://";
    private const string Https = "https://";
    private string[] _sources;
    private readonly string[] _acceptedSources = { Rtmp, Rtsp, Http, Https };

    public Worker(ILogger<Worker> logger) {
        _logger = logger;
    }

    private void HandleStreams() {
        // > Handle Azure video blob

        foreach (string source in _sources) {
            StreamConnection streamConnection = new(source);

            streamConnection.OnConnectionEstablished += () => {
                FrameSender frameSender = new FrameSender(streamConnection);
                frameSender.SendFrames("http://localhost:5000/inference");
            };

            // streamConnection.OnConnectionBroken += () => {
            //     frameSender.Dispose();
            // };

            // CvInvoke.DestroyAllWindows();
            // streamReader.Dispose();
        }
    }

    private bool ValidateConfig() {
        bool isSourcesAccepted = _acceptedSources.Any(
            acceptedSource => _sources.Any(
                source => source
                    .ToLower()
                    .StartsWith(acceptedSource)
            )
        );

        if (!isSourcesAccepted) {
            return false;
        }

        return true;
    }
    
    private void Dispose() {

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        /*
         * - Gain access to config (API url)
         * - Start RTMP/RTSP server in separate thread (seems complicated)
         * - Listen to input from source (w/ API for Blob input & RTMP/RTSP server)
         * - Initiate handle stream on input trigger
         */

        _sources = new[]{ "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5" };
        // string[] sources = { "assets/src-traffic.mp4" };

        // string[] streamSources = sources.Where(
        //     source => source.ToLower().StartsWith(Rtmp) || source.ToLower().StartsWith(Rtsp)
        // ).ToArray();

        if (ValidateConfig()) {
            _logger.LogError("The input format is not accepted");
            return;
        }
        
        HandleStreams();

        // while (!stoppingToken.IsCancellationRequested) {
        //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     
        //     await Task.Delay(1000, stoppingToken);
        // }
    }
}