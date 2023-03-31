﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
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
        
        _entryPointService.Stop();
    }
}