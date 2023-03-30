using System;
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
    private readonly IPubSub _pubSub;

    public StreamService(
      IStreamReceiver streamReceiver,
      IStreamSender streamSender,
      IPubSub pubSub)
    {
        _streamReceiver = streamReceiver;
        _streamSender = streamSender;
        _pubSub = pubSub;
    }

    public void HandleStream(string source, string targetUrl, string prepareUrl)
    {
        Guard.Against.NullOrWhiteSpace(source, nameof(source));

        _streamSender.PrepareTarget();
        _streamReceiver.ConnectStreamBySource(source);

        _streamReceiver.OnConnectionEstablished += () =>
        {
            _streamSender.SendStreamToEndpoint(_streamReceiver, targetUrl, prepareUrl);
        };

        _streamReceiver.OnConnectionBroken += () =>
        {
            
        };

        _streamReceiver.OnConnectionTimeout += () =>
        {
            _streamReceiver.Dispose();
            _streamSender.Dispose();
        };

        _streamSender.OnPredictionResult += async result =>
        {
            await _pubSub.Send(result);
            
            // Store in db
        };
    }
}
