using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Worker;

/// <summary>
/// Base BackgroundService worker class that calls entrypoint service in a loop
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILoggerAdapter<Worker> _logger;
    private readonly IEntryPointService _entryPointService;
    private readonly WorkerSettings _settings;

    public Worker(ILoggerAdapter<Worker> logger,
        IEntryPointService entryPointService,
        IOptions<WorkerSettings> workerSettingsOptions)
    {
        _logger = logger;
        _entryPointService = entryPointService;
        _settings = workerSettingsOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            int.TryParse(Environment.GetEnvironmentVariable("SessionId"), out var sessionId);
            
            await _entryPointService.Execute(sessionId, stoppingToken);
            await Task.Delay(_settings.DelayMilliseconds, stoppingToken);
        }
        
        _entryPointService.Stop();
    }
}
