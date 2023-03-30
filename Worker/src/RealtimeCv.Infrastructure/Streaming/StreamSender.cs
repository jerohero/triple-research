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
    public event Action<object?>? OnPredictionResult;
    
    private readonly ILoggerAdapter<StreamSender> _logger;
    private IStreamReceiver? _streamReceiver;
    private Thread? _sendThread;
    private Thread? _prepareThread;
    private string? _targetUrl;
    private string? _prepareUrl;
    private bool _isPrepared;
    private readonly IHttpService _httpService;

    public StreamSender(
      ILoggerAdapter<StreamSender> logger,
      IHttpService httpService
    )
    {
        _logger = logger;
        _httpService = httpService;
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
    
    public void PrepareTarget()
    {
        if (_prepareThread is { IsAlive: true })
        {
            return;
        }
        
        _prepareThread = new Thread(PrepareEndpoint)
        {
            IsBackground = true
        };

        _prepareThread.Start();
    }

    private async void SendFramesToTarget()
    {
        Guard.Against.NullOrWhiteSpace(_targetUrl, nameof(_targetUrl));
        Guard.Against.Null(_streamReceiver, nameof(_streamReceiver));

        var frameCount = 0;

        while (!_streamReceiver.Frame.Empty())
        {
            frameCount++;

            var now = DateTime.UtcNow;

            try
            {
                var res = await _httpService.PostFile(_targetUrl, _streamReceiver.Frame.ToBytes());
                var results = await res.Content.ReadFromJsonAsync<object>();
                
                OnPredictionResult?.Invoke(results);

                var time = (DateTime.UtcNow - now).TotalSeconds;

                _logger.LogInformation($"Sent frame {frameCount}. Took {time} seconds.");
            }
            catch (HttpRequestException)
            {
                if (_isPrepared)
                {
                    // TODO throw error, as this would mean the inference API crashed
                }
                
                PrepareEndpoint();
                Thread.Sleep(5000);
            }
        }
    }

    private async void PrepareEndpoint()
    {
        var didPrepare = false;
        
        // Executes in a loop because the target may not have started yet
        while (!didPrepare)
        {
            try
            {
                await _httpService.Post(_prepareUrl);

                didPrepare = true;
            }
            catch (HttpRequestException)
            {
                didPrepare = false;
                Thread.Sleep(5000);
            }
        }

        _isPrepared = didPrepare;
    }

    public void Dispose()
    {
        _sendThread?.Join();
        _prepareThread?.Join();
    }
}
