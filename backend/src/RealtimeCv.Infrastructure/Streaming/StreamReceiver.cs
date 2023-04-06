using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Ardalis.GuardClauses;
using Newtonsoft.Json;
using OpenCvSharp;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Streaming;

/// <summary>
/// Connects to a stream input and converts it into frames.
/// </summary>
public class StreamReceiver : IStreamReceiver, IDisposable
{
    public Mat Frame { get; private set; }
    public event Action OnConnectionEstablished;
    public event Action OnConnectionBroken;
    public event Action? OnConnectionTimeout;

    private const int DefaultFps = 30;
    private const int SecondsBetweenAttempts = 5;
    private readonly ILoggerAdapter<StreamReceiver> _logger;
    private string? _source;
    private int? _fps;
    private VideoCapture? _capture;
    private Thread? _updateThread;
    private Thread? _pollThread;
    private int? _secondsBeforeTimeout;

    public StreamReceiver(ILoggerAdapter<StreamReceiver> logger)
    {
        _logger = logger;
        Frame = new Mat();

        OnConnectionEstablished += ReadStream;
        OnConnectionBroken += PollStream;
    }

    public void ConnectStreamBySource(string source, int secondsBeforeTimeout = 15)
    {
        Guard.Against.NullOrWhiteSpace(source, nameof(source));

        _secondsBeforeTimeout = secondsBeforeTimeout;
        _source = source;

        _pollThread = new Thread(PollStream)
        {
            IsBackground = true
        };

        _pollThread.Start();
    }

    public bool CheckConnection(string source)
    {
        // return CheckConnectionOpenCv(source);
        return CheckConnectionFfmpeg(source);
    }

    // For research
    private bool CheckConnectionOpenCv(string source)
    {
        // Due to its synchronous nature this tends to take very long (10+ seconds per URL in larger loops for some reason) and create false results
        
        VideoCapture capture = new(source);
        var isOpened = capture.IsOpened();
        capture.Release();

        return isOpened;
    }

    private bool CheckConnectionFfmpeg(string source)
    {
        // ~1 second per URL. I did find that ffprobe tends to return empty results at times, even though the stream is active
        
        var ffprobePath = "C:/ffmpeg/bin/ffprobe.exe";
        
        var process = new Process();
        process.StartInfo.FileName = ffprobePath;
        process.StartInfo.Arguments = $"-v quiet -print_format json -show_streams {source}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        
        process.Start();
        var output = JsonConvert.DeserializeObject(process.StandardOutput.ReadToEnd());
        process.Close();
        
        return  !output?.ToString()?.Equals("{}") ?? false;
    }

    private void PollStream()
    {
        Guard.Against.Null(_source);

        VideoCapture capture = new(_source);

        var failedAttempts = 0;

        while (!capture.IsOpened())
        {
            _logger.LogInformation($"Failed to open {_source} on attempt {failedAttempts}. Retrying in {SecondsBetweenAttempts} seconds.");
            
            HandleTimeout(failedAttempts);
            
            failedAttempts++;
            
            Thread.Sleep(1000 * SecondsBetweenAttempts);
            
            capture = new VideoCapture(_source);
        }

        capture.Set(VideoCaptureProperties.BufferSize, 2);
        _capture = capture;

        OnConnectionEstablished();
    }

    private void HandleTimeout(int failedAttempts)
    {
        if (failedAttempts * SecondsBetweenAttempts >= _secondsBeforeTimeout)
        {
            OnConnectionTimeout?.Invoke();
        }
    }

    private void ReadStream()
    {
        if (_capture is null || !_capture.IsOpened())
        {
            _logger.LogInformation("Stream is inactive");
            return;
        }

        _fps = (int)_capture.Get(VideoCaptureProperties.Fps);
        _capture.Read(Frame); // guarantee first frame

        _updateThread = new Thread(Update)
        {
            IsBackground = true
        };

        _updateThread.Start();
    }

    private void Update()
    {
        // Read next stream frame in a daemon thread
        while (_capture is not null && _capture.IsOpened())
        {
            _capture.Grab();

            Mat frame = new();
            _capture.Retrieve(frame);
            Frame = frame;

            if (frame.Empty())
            {
                break;
            }

            Thread.Sleep((int)TimeSpan.FromSeconds(1 / _fps ?? DefaultFps).TotalMilliseconds); // wait time
        }

        _logger.LogInformation("Connection broken");

        OnConnectionBroken();
    }

    public void Dispose()
    {
        _updateThread?.Join();
        _pollThread?.Join();
        _capture?.Dispose();
        Frame.Dispose();
    }
}
