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
    private const string ConnStringName = "AzureWebPubSub";
    private WebPubSubServiceClient? _serviceClient;

    public PubSub(
      ILoggerAdapter<PubSub> logger
    )
    {
        _logger = logger;
    }

    public async Task Send(object message, string group, string hub)
    {
        Guard.Against.Null(message, nameof(message));
        Guard.Against.NullOrEmpty(hub, nameof(hub));
        
        Connect(hub);

        await _serviceClient!.SendToGroupAsync(group, RequestContent.Create(message), ContentType.ApplicationJson);
    }

    public async Task<Uri> Negotiate(string hub, string group)
    {
        Guard.Against.NullOrEmpty(hub, nameof(hub));
        
        Connect(hub);
        
        return await _serviceClient!.GetClientAccessUriAsync(groups: new[] { group });
    }

    private void Connect(string hub)
    {
        if (_serviceClient?.Hub != hub)
        {
            _serviceClient = new WebPubSubServiceClient(Environment.GetEnvironmentVariable(ConnStringName), hub);
        }
    }
}
