using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Worker.Services;

/// <summary>
/// Core business logic entrypoint service that manages the stream vision flow.
/// </summary>
public class EntryPointService : IEntryPointService
{
    private readonly ILoggerAdapter<EntryPointService> _logger;
    private readonly IStreamService _streamService;
    private readonly ISessionHandlerService _sessionHandlerService;
    private readonly IPubSub _pubSub;
    private readonly IBlob _blob;
    private bool _isRunning;

    public EntryPointService(ILoggerAdapter<EntryPointService> logger,
        IStreamService streamService,
        ISessionHandlerService sessionHandlerService,
        IPubSub pubSub,
        IBlob blob)
    {
        _logger = logger;
        _streamService = streamService;
        _sessionHandlerService = sessionHandlerService;
        _pubSub = pubSub;
        _blob = blob;
    }

    public async Task Execute(int sessionId, CancellationToken stoppingToken)
    {
        if (_isRunning)
        {
            return;
        }
        
        _isRunning = true;
        
        _logger.LogInformation("{service} running at: {time}", nameof(EntryPointService), DateTimeOffset.Now);
        
        var targetUrl = "http://127.0.0.1:5000";

        var session = await _sessionHandlerService.SetSessionActive(sessionId);

        var model = await _sessionHandlerService.GetSessionTrainedModel(sessionId);
        var modelUri = _blob.GetBlobUri(model.Name, "trained-model");

        _streamService.HandleStream(session, modelUri, targetUrl);
        
        _streamService.OnStreamEnded += () =>
        {
            _sessionHandlerService.EndSession(session.Id);
            // _isRunning = false;
        };
    }
    
    public void Stop()
    {
        _isRunning = false;
        _streamService.Dispose();
    }
}
