using System;
using System.Threading;
using Ardalis.GuardClauses;
using OpenCvSharp;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Streaming;

/// <summary>
/// Connects to a stream input and converts it into frames
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

    private void PollStream()
    {
        Guard.Against.Null(_source);

        VideoCapture capture = new(_source);

        var failedAttempts = 0;

        while (!capture.IsOpened())
        {
            _logger.LogInformation($"Failed to open {_source} on attempt {failedAttempts}. Retrying in {SecondsBetweenAttempts} sec..");
            
            if (failedAttempts * SecondsBetweenAttempts >= _secondsBeforeTimeout)
            {
                OnConnectionTimeout?.Invoke();
            }
            
            failedAttempts++;
            
            Thread.Sleep(1000 * SecondsBetweenAttempts);
            
            capture = new VideoCapture(_source);
        }

        capture.Set(VideoCaptureProperties.BufferSize, 2);
        _capture = capture;

        OnConnectionEstablished();
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
