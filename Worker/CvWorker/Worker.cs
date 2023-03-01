using System.Text.RegularExpressions;
using CvWorker.Util;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace CvWorker;

public class Worker : BackgroundService {
    private readonly ILogger<Worker> _logger;
    private readonly string[] _acceptedSources = { "rtmp://", "rtsp://", "http://", "https://" };

    private Thread[] _threads;
    private Mat[] _frames;
    private VideoCapture[] _captures;
    private int _imgSize;
    private int _fps;

    public Worker(ILogger<Worker> logger) {
        _logger = logger;
    }
    
    private void HandleStream(string[] sources) {
        // > Maybe handle multiple sources so one container can handle multiple streams where speed is less trivial
        
        // source = StringUtil.SpecialCharsToUnderscore(source);
        
        bool isSourceAccepted = _acceptedSources.Any(
            acceptedSource => sources.Any(
                source => source
                    .ToLower()
                    .StartsWith(acceptedSource)
            )
        );

        if (!isSourceAccepted) {
            _logger.LogError("The input format is not accepted");
            return;
        }
        
        // > Handle Azure video blob if applicable

        foreach (string source in sources) {
            Stream stream = new(source);
            
            while (!stream.Frame.IsEmpty) {
                CvInvoke.Imshow(source, stream.Frame);
                CvInvoke.WaitKey(1);
            }
            
            CvInvoke.DestroyAllWindows();
            stream.Dispose();
        }
    }

    private void Dispose() {
        foreach (Thread thread in _threads) {
            thread.Join();
        }
        foreach (Mat frame in _frames) {
            frame.Dispose();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        string[] sources = { "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5" };
        // string[] sources = { "assets/src-traffic.mp4" };
        HandleStream(sources);
        
        // while (!stoppingToken.IsCancellationRequested) {
        //     _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
        //     await Task.Delay(1000, stoppingToken);
        // }
    }
}