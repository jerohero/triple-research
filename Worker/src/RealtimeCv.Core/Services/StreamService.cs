using Ardalis.GuardClauses;
using System.Threading.Tasks;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Services;

/// <summary>
/// A simple service that fetches a URL and returns a UrlStatusHistory instance with the result
/// </summary>
public class StreamService : IStreamService
{
  private readonly IStreamReceiver _streamReceiver;
  private readonly IStreamSender _streamSender;
  private readonly IHttpService _httpService;
  
  public StreamService(
    IStreamReceiver streamReceiver,
    IStreamSender streamSender,
    IHttpService httpService)
  {
    _streamReceiver = streamReceiver;
    _streamSender = streamSender;
    _httpService = httpService;
  }

  public void HandleStream(string source, string target)
  {
    Guard.Against.NullOrWhiteSpace(source, nameof(source));

    _streamReceiver.ConnectStreamBySource(source);

    _streamReceiver.OnConnectionEstablished += async () =>
    {
      await PrepareInference();
      _streamSender.SendStreamToEndpoint(_streamReceiver, target);
    };

    _streamReceiver.OnConnectionBroken += () =>
    {
      // _streamReceiver.Dispose();
      // _streamSender.Dispose();
    };
  }

  // TODO move?
  private async Task PrepareInference()
  {
    await _httpService.PostAsync("http://localhost:5000/start");
  }
}
