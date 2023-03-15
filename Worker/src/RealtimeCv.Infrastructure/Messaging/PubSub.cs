using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Messaging.WebPubSub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Messaging;

public class PubSub : IPubSub
{
  private readonly ILoggerAdapter<PubSub> _logger;
  private readonly IConfiguration _configuration;
  private const string HubName = "predictions";
  private const string ConnStringName = "AzureWebPubSub";
  private WebPubSubServiceClient? _serviceClient;

  public PubSub(
    IConfiguration configuration,
    ILoggerAdapter<PubSub> logger
  )
  {
    _configuration = configuration;
    _logger = logger;
  }

  public async Task Init()
  {
    _serviceClient = new WebPubSubServiceClient(_configuration.GetConnectionString(ConnStringName), HubName);
    
    // TODO either store somewhere the client can access it or turn it into azure function negotiate func
    _logger.LogInformation("URI: " + await _serviceClient.GetClientAccessUriAsync(TimeSpan.FromHours(72)));
  }

  public async Task Send(object message)
  {
    if (_serviceClient is null)
    {
      await Init();
    }

    await _serviceClient!.SendToAllAsync(RequestContent.Create(message), ContentType.ApplicationJson);
  }
}
