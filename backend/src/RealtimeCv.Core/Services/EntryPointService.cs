using System;
using System.Threading;
using System.Threading.Tasks;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Services;

/// <summary>
/// Core business logic entrypoint service that manages the stream vision flow.
/// </summary>
public class EntryPointService : IEntryPointService
{
    private readonly ILoggerAdapter<EntryPointService> _logger;
    private readonly IStreamService _streamService;
    private readonly IServiceLocator _serviceScopeFactoryLocator;
    private readonly IPubSub _pubSub;
    private bool _isRunning;

    public EntryPointService(ILoggerAdapter<EntryPointService> logger,
        IStreamService streamService,
        IServiceLocator serviceScopeFactoryLocator,
        IPubSub pubSub)
    {
        _logger = logger;
        _streamService = streamService;
        _serviceScopeFactoryLocator = serviceScopeFactoryLocator;
        _pubSub = pubSub;
    }

    public async Task Execute(CancellationToken stoppingToken)
    {
        if (_isRunning)
        {
            return;
        }
        
        _isRunning = true;
        
        _logger.LogInformation("{service} running at: {time}", nameof(EntryPointService), DateTimeOffset.Now);

        var source = "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5";
        var targetUrl = "http://127.0.0.1:5000";

        // EF Requires a scope so we are creating one per execution here
        // using var scope = _serviceScopeFactoryLocator.CreateScope();

        _streamService.HandleStream(source, targetUrl);
    }
    
    public void Stop()
    {
        _isRunning = false;
        _streamService.Dispose();
    }
}
