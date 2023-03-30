using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Worker;

/// <summary>
/// The Worker is a BackgroundService that is executed periodically
/// It should not contain any business logic but should call an entrypoint service that
/// execute once per time period.
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILoggerAdapter<Worker> _logger;
    private readonly IEntryPointService _entryPointService;
    private readonly WorkerSettings _settings;

    public Worker(ILoggerAdapter<Worker> logger,
        IEntryPointService entryPointService,
        WorkerSettings settings)
    {
        _logger = logger;
        _entryPointService = entryPointService;
        _settings = settings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _entryPointService.Execute(stoppingToken);
            await Task.Delay(_settings.DelayMilliseconds, stoppingToken);
        }
    }
}
