#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using RealtimeCv.Core.Interfaces;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace RealtimeCv.Infrastructure.Streaming;

/// <summary>
/// 
/// </summary>
public class StreamReceiver : IStreamReceiver
{
  private readonly ILoggerAdapter<StreamReceiver> _logger;
  public Mat Frame;
  private readonly string _source;
  private int _fps;
  private VideoCapture? _capture;
  private Thread? _updateThread;
  private readonly Thread? _pollThread;
  public event ConnectionEventHandler OnConnectionEstablished;
  public event ConnectionEventHandler OnConnectionBroken;

  public StreamReceiver(
    ILoggerAdapter<StreamReceiver> logger
  ) {
    _logger = logger;
  }

  public void GetStreamFromSource(string source)
  {
    Guard.Against.NullOrWhiteSpace(source, nameof(source));
    
    _source = source;
    Frame = new Mat();

    _pollThread = new Thread(PollStream) {
      IsBackground = true
    };
        
    _pollThread.Start();

    OnConnectionEstablished += ReadStream;
    OnConnectionBroken += PollStream;
  }
  
  private void PollStream() {
    VideoCapture capture = new(_source);

    while (!capture.IsOpened) {
      _logger.LogInformation($"Failed to open { _source }. Retrying in 10 sec..");
      Thread.Sleep(1000 * 10);
      capture = new VideoCapture(_source);
    }

    capture.Set(CapProp.Buffersize, 2);
    _capture = capture;
        
    OnConnectionEstablished();
  }

  private void ReadStream() {
    // int w = (int)_capture.Get(CapProp.FrameWidth);
    // int h = (int)_capture.Get(CapProp.FrameHeight);
    if (!_capture!.IsOpened) {
      _logger.LogInformation("Stream is inactive");
      return;
    }
        
    _fps = (int)_capture.Get(CapProp.Fps);
    // Console.WriteLine($"Success { w }x{ h } at { _fps } FPS");
    _capture.Read(Frame); // guarantee first frame

    _updateThread = new Thread(Update) {
      IsBackground = true
    };
        
    _updateThread.Start();
  }

  private void Update()
  {
    // Read next stream frame in a daemon thread
    while (_capture!.IsOpened) {
      _capture.Grab();
            
      Mat frame = new();
      _capture.Retrieve(frame);
      Frame = frame;

      Thread.Sleep(1000 / _fps); // wait time
    }

    OnConnectionBroken();
  }
}
