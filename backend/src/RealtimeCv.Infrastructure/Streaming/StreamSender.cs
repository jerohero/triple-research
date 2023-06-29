using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using Ardalis.GuardClauses;
using Newtonsoft.Json;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Infrastructure.Data.Config;
using JsonException = System.Text.Json.JsonException;

namespace RealtimeCv.Infrastructure.Streaming;

/// <summary>
/// Takes the latest stream frames and sends it to the inference API.
/// </summary>
public class StreamSender : IStreamSender, IDisposable
{
    public event Action<object?>? OnPredictionResult;
    public event Action? OnConnectionTimeout;
    
    private readonly ILoggerAdapter<StreamSender> _logger;
    private IStreamReceiver? _streamReceiver;
    private Thread? _sendThread;
    private Thread? _prepareThread;
    private string? _targetUrl;
    private string? _prepareUrl;
    private bool _isPrepared;
    private readonly IHttpService _httpService;
    private int? _secondsBeforeTimeout;

    public StreamSender(
      ILoggerAdapter<StreamSender> logger,
      IHttpService httpService
    )
    {
        _logger = logger;
        _httpService = httpService;
    }

    public void PrepareTarget(string prepareUrl, string modelName, int secondsBeforeTimeout = 180)
    {
        Guard.Against.NullOrEmpty(prepareUrl);
        
        _prepareUrl = prepareUrl;
        _secondsBeforeTimeout = secondsBeforeTimeout;
        
        if (_prepareThread is { IsAlive: true })
        {
            _logger.LogInformation("Prepare thread is already running.");
            return;
        }
        
        _prepareThread = new Thread(() => AttemptPrepareTarget(modelName))
        {
            IsBackground = true
        };

        _prepareThread.Start();
    }
    
    public void SendStreamToEndpoint(IStreamReceiver streamReceiver, string targetUrl)
    {
        Guard.Against.NullOrEmpty(targetUrl);
        Guard.Against.NullOrEmpty(_prepareUrl);
        
        _streamReceiver = streamReceiver;
        _targetUrl = targetUrl;

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

        var frameCount = 0;

        while (!_streamReceiver.Frame.Empty())
        {
            frameCount++;

            var now = DateTime.UtcNow;

            if (!_isPrepared)
            {
                _logger.LogInformation("Target is still preparing. Retrying in 5 seconds.");
                Thread.Sleep(Constants.DefaultActionDelayMs);

                continue;
            }

            try
            {
                var payload = await _httpService.ImageToHttpContent(_streamReceiver.Frame.ToBytes());
                var res = await _httpService.Post(_targetUrl, payload);

                var results = await res.Content.ReadFromJsonAsync<object>();

                OnPredictionResult?.Invoke(results);

                var time = (DateTime.UtcNow - now).TotalSeconds;

                _logger.LogInformation($"Sent frame {frameCount}. Took {time} seconds.");
            }
            catch (HttpRequestException)
            {
                _logger.LogInformation("Something went wrong");
            }
            catch (JsonException)
            {
                _logger.LogInformation("Result was not in JSON format.");
            }
        }
    }

    private async void AttemptPrepareTarget(string modelName)
    {
        var didPrepare = false;
        
        var failedAttempts = 0;
        
        // Executes in a loop because the target may not have started yet
        while (!didPrepare)
        {
            try
            {
                var content = _httpService.StringToHttpContent(modelName);
                var res = await _httpService.Post(_prepareUrl, content);
                
                _logger.LogInformation("Prepared target.");
                
                if (!res.IsSuccessStatusCode)
                {
                    didPrepare = false;
                    failedAttempts = HandleFailedPrepare(failedAttempts);
                    
                    continue;
                }
                
                didPrepare = true;
            }
            catch (HttpRequestException)
            {
                didPrepare = false;
                failedAttempts = HandleFailedPrepare(failedAttempts);
            }
        }

        _isPrepared = didPrepare;
    }
    
    private int HandleFailedPrepare(int failedAttempts)
    {
        _logger.LogInformation("Failed to prepare target. Retrying in 5 seconds.");
                
        HandleTimeout(failedAttempts);
        failedAttempts++;
        
        Thread.Sleep(Constants.DefaultActionDelayMs);

        return failedAttempts;
    }
    
    private void HandleTimeout(int failedAttempts)
    {
        if (failedAttempts * Constants.DefaultActionDelayMs >= _secondsBeforeTimeout)
        {
            OnConnectionTimeout?.Invoke();
        }
    }

    public void Dispose()
    {
        _sendThread?.Join();
        _prepareThread?.Join();
    }
}
