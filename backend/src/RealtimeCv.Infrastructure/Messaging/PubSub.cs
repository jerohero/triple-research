using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Azure.Core;
using Azure.Messaging.WebPubSub;
using Microsoft.Extensions.Configuration;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Messaging;

/// <summary>
/// Handles all Azure Web PubSub related tasks for asynchronous messaging.
/// </summary>
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
        _serviceClient = new WebPubSubServiceClient(_configuration.GetConnectionString(ConnStringName), HubName);
    }

    public async Task Send(object message)
    {
        Guard.Against.Null(_serviceClient);

        // TODO: To SendToGroup
        await _serviceClient!.SendToAllAsync(RequestContent.Create(message), ContentType.ApplicationJson);
    }
}
