using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Azure.Core;
using Azure.Messaging.WebPubSub;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Messaging;

/// <summary>
/// Handles all Azure Web PubSub related tasks for asynchronous messaging.
/// </summary>
public class Queue : IQueue
{
    private readonly ILoggerAdapter<PubSub> _logger;
    private readonly IConfiguration _configuration;
    private const string ConnStringName = "AzureWebJobsStorage";

    public Queue(
      IConfiguration configuration,
      ILoggerAdapter<PubSub> logger
    )
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendMessage(string queueName, object message)
    {
        var connString = Environment.GetEnvironmentVariable(ConnStringName); // TODO need to call this in a way that it would work in both the worker and function app
        var messageString = JsonConvert.SerializeObject(message);

        var queueClient = new QueueClient(connString, queueName, new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64
        });

        await queueClient.CreateIfNotExistsAsync();

        if (await queueClient.ExistsAsync())
        {
            await queueClient.SendMessageAsync(messageString);
        }
    }
}
