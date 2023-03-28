using System.Threading.Tasks;
using Ardalis.GuardClauses;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Services;

/// <summary>
/// 
/// </summary>
public class StreamService : IStreamService
{
    private readonly IStreamReceiver _streamReceiver;
    private readonly IStreamSender _streamSender;

    public StreamService(
      IStreamReceiver streamReceiver,
      IStreamSender streamSender)
    {
        _streamReceiver = streamReceiver;
        _streamSender = streamSender;
    }

    public void HandleStream(string source, string targetUrl, string prepareUrl)
    {
        Guard.Against.NullOrWhiteSpace(source, nameof(source));

        _streamReceiver.ConnectStreamBySource(source);

        _streamReceiver.OnConnectionEstablished += () =>
        {
            _streamSender.SendStreamToEndpoint(_streamReceiver, targetUrl, prepareUrl);
        };

        _streamReceiver.OnConnectionBroken += () =>
        {
            // _streamReceiver.Dispose();
            // _streamSender.Dispose();
        };
    }
}
