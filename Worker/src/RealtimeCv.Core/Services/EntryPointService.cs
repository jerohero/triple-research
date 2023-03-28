using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Settings;

namespace RealtimeCv.Core.Services;

/// <summary>
/// An example service that performs business logic
/// </summary>
public class EntryPointService : IEntryPointService
{
  private readonly ILoggerAdapter<EntryPointService> _logger;
  private readonly EntryPointSettings _settings;
  private readonly IQueueReceiver _queueReceiver;
  private readonly IQueueSender _queueSender;
  private readonly IStreamService _streamService;
  private readonly IServiceLocator _serviceScopeFactoryLocator;
  private readonly IPubSub _pubSub;

  public EntryPointService(ILoggerAdapter<EntryPointService> logger,
      EntryPointSettings settings,
      IQueueReceiver queueReceiver,
      IQueueSender queueSender,
      IStreamService streamService,
      IServiceLocator serviceScopeFactoryLocator,
      IPubSub pubSub)
  {
    _logger = logger;
    _settings = settings;
    _queueReceiver = queueReceiver;
    _queueSender = queueSender;
    _streamService = streamService;
    _serviceScopeFactoryLocator = serviceScopeFactoryLocator;
    _pubSub = pubSub;
  }

  public async Task ExecuteAsync()
  {
    _logger.LogInformation("{service} running at: {time}", nameof(EntryPointService), DateTimeOffset.Now);

    await Task.CompletedTask; // temp

    string[] sources = { "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5" };

    // TODO Note that this is called in loops, so avoid having hundreds of threads doing the same thing
    // TODO we can do this by polling queue messages in the loop, and only creating the stream threads when one is received
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

      foreach (string source in sources)
      {
        _streamService.HandleStream(source, "http://127.0.0.1:5000/inference", "http://127.0.0.1:5000/start");
      }

      // Delete below
      // // read from the queue
      // string message = await _queueReceiver.GetMessageFromQueue(_settings.ReceivingQueueName);
      // if (String.IsNullOrEmpty(message)) return;
      //
      // // check 1 URL in the message
      // var statusHistory = await _urlStatusChecker.CheckUrlAsync(message, "");
      //
      // // record HTTP status / response time / maybe existence of keyword in database
      // repository.Add(statusHistory);
      //
      // _logger.LogInformation(statusHistory.ToString());
    }
#pragma warning disable CA1031 // Do not catch general exception types
    catch (Exception ex)
    {
      _logger.LogError(ex, $"{nameof(EntryPointService)}.{nameof(ExecuteAsync)} threw an exception.");
      // TODO: Decide if you want to re-throw which will crash the worker service
      //throw;
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
}
