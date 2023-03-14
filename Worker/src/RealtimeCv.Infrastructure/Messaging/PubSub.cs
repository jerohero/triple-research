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
  
  public PubSub(IConfiguration configuration, ILoggerAdapter<PubSub> logger)
  {
    _configuration = configuration;
    _logger = logger;
  }
  
  public void Get()
  {
    string connectionString =
      "Endpoint=https://triplejbpubsub.webpubsub.azure.com;AccessKey=D4Pa2W274942Lwx+Q79RdJWP/rUzodEfPWCh30MZz7M=;Version=1.0;";
    string hubName = "predictions";
    
    WebPubSubServiceClient serviceClient = new(connectionString, hubName);

    serviceClient.SendToAll(RequestContent.Create(new { Foo = "Hello", }), ContentType.ApplicationJson);
  }
}
