using System;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using OpenCvSharp.Extensions;
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
  private string? _prepareUrl;
  private IHttpService _httpService;
  private IPubSub _pubSub;
  
  public StreamSender(
    ILoggerAdapter<StreamSender> logger,
    IHttpService httpService,
    IPubSub pubSub
  )
  {
    _logger = logger;
    _httpService = httpService;
    _pubSub = pubSub;
  }
  
  public void SendStreamToEndpoint(IStreamReceiver streamReceiver, string targetUrl, string prepareUrl)
  {
    _streamReceiver = streamReceiver;
    _targetUrl = targetUrl;
    _prepareUrl = prepareUrl;
    
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

    await PrepareEndpoint();
    
    while (!_streamReceiver.Frame.Empty())
    {
      frameCount++;

      DateTime now = DateTime.UtcNow;

      try
      {
        HttpResponseMessage res = await _httpService.PostFileAsync(_targetUrl, _streamReceiver.Frame.ToBytes());
        object? results = await res.Content.ReadFromJsonAsync<object>();

        await _pubSub.Send(results);

        double time =  (DateTime.UtcNow - now).TotalSeconds;
      
        _logger.LogInformation($"Sent frame { frameCount }. Took { time } seconds.");
      }
      catch (HttpRequestException)
      {
        // TODO it may be better to prepare the endpoint at the start, as this will result in a larger loss of frames
        await PrepareEndpoint();
        
        _logger.LogInformation("Endpoint was not prepared. Preparing..");
      }

      // Still seems too slow? might want to test with video
      // float fps = (1f/30f);
      // int fpsMs = (int) (fps * 1000f);
      // Thread.Sleep(fpsMs); // wait time
    }
  }

  private async Task PrepareEndpoint()
  {
    await _httpService.PostAsync(_prepareUrl);
  }
  
  public void Dispose()
  {
    _sendThread?.Join();
  }
}
