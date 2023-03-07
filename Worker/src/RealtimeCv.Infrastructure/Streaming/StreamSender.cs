using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Emgu.CV;
using Emgu.CV.Structure;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Streaming;

/// <summary>
/// Takes the latest stream frames and sends it to an inference API
/// </summary>
public class StreamSender : IStreamSender, IDisposable
{
  private readonly ILoggerAdapter<StreamSender> _logger;
  private IStreamReceiver? _streamReceiver;
  private Thread? _sendThread;
  private string? _targetUrl;
  private IHttpService _httpService;
  
  public StreamSender(
    ILoggerAdapter<StreamSender> logger,
    IHttpService httpService
  )
  {
    _logger = logger;
    _httpService = httpService;
  }
  
  public void SendStreamToEndpoint(IStreamReceiver streamReceiver, string url)
  {
    _streamReceiver = streamReceiver;
    _targetUrl = url;
        
    _sendThread = new Thread(SendFramesToTarget)
    {
      IsBackground = true
    };
        
    _sendThread.Start();
  }

  private async void SendFramesToTarget()
  {
    Guard.Against.NullOrWhiteSpace(_targetUrl, nameof(_targetUrl));
    Guard.Against.Null(_streamReceiver, nameof(_streamReceiver));

    int frameCount = 0;

    while (!_streamReceiver.Frame.IsEmpty)
    {
      frameCount++;
      
      // CvInvoke.Imshow("frames", _streamReceiver.Frame);
      // CvInvoke.WaitKey(1);
      byte[] image = _streamReceiver.Frame.ToImage<Bgr, byte>().ToJpegData();
      
      await _httpService.PostFileAsync(_targetUrl, image);
      
      _logger.LogInformation("Send frame " + frameCount);
      // Still seems too slow? might want to test with video
      // float fps = (1f/30f);
      // int fpsMs = (int) (fps * 1000f);
      // Thread.Sleep(fpsMs); // wait time
    }

  }

  public void Dispose()
  {
    _sendThread?.Join();
  }
}
