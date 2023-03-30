using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Settings;

namespace RealtimeCv.Core.Services;

public class EntryPointService : IEntryPointService
{
    private readonly ILoggerAdapter<EntryPointService> _logger;
    private readonly EntryPointSettings _settings;
    private readonly IStreamService _streamService;
    private readonly IServiceLocator _serviceScopeFactoryLocator;
    private readonly IPubSub _pubSub;
    private bool _isRunning;

    public EntryPointService(ILoggerAdapter<EntryPointService> logger,
        EntryPointSettings settings,
        IStreamService streamService,
        IServiceLocator serviceScopeFactoryLocator,
        IPubSub pubSub)
    {
        _logger = logger;
        _settings = settings;
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

        string[] sources = { "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5" };

        // TODO Note that this is called in loops, so avoid having hundreds of threads doing the same thing
        // TODO we can do this by polling queue messages in the loop, and only executing the below code when one is received
        try
        {
            // EF Requires a scope so we are creating one per execution here
            using var scope = _serviceScopeFactoryLocator.CreateScope();
            // var repository = scope.ServiceProvider.GetService<IRepository>();
            //
            // VisionSet vs = new()
            // {
            //   Name = "Test",
            //   Models = new List<string>(),
            //   Sources = new List<string> { "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5" },
            // };
            //
            // repository.Add(vs);

            await _pubSub.Init();

            foreach (var source in sources)
            {
                _streamService.HandleStream(source, "http://127.0.0.1:5000/inference", "http://127.0.0.1:5000/start");
            }
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(EntryPointService)}.{nameof(Execute)} threw an exception.");
            // TODO: Decide if you want to re-throw which will crash the worker service
            //throw;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}
