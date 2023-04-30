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

    public async Task Send(object message, string hub)
    {
        Guard.Against.Null(message, nameof(message));
        Guard.Against.NullOrEmpty(hub, nameof(hub));
        
        Connect(hub);
        
        await _serviceClient!.SendToAllAsync(RequestContent.Create(message), ContentType.ApplicationJson);
    }

    public async Task<Uri> Negotiate(string hub)
    {
        Guard.Against.NullOrEmpty(hub, nameof(hub));
        
        Connect(hub);

        return await _serviceClient!.GetClientAccessUriAsync(TimeSpan.FromHours(24));
    }

    private void Connect(string podName)
    {
        if (_serviceClient?.Hub != podName)
        {
            _serviceClient = new WebPubSubServiceClient(_configuration.GetConnectionString(ConnStringName), podName);
        }
    }
}
