using System;
using Ardalis.GuardClauses;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Services;

/// <summary>
/// Service that oversees the process of consuming the input, sending it to the inference API and publishing the results.
/// </summary>
public class StreamInferenceService : IStreamInferenceService, IDisposable
{
    private readonly IStreamReceiver _streamReceiver;
    private readonly IStreamSender _streamSender;
    private readonly IPubSub _pubSub;

    public StreamInferenceService(
      IStreamReceiver streamReceiver,
      IStreamSender streamSender,
      IPubSub pubSub)
    {
        _streamReceiver = streamReceiver;
        _streamSender = streamSender;
        _pubSub = pubSub;
    }

    public void HandleStream(string source, string targetUrl)
    {
        Guard.Against.NullOrWhiteSpace(source, nameof(source));
        Guard.Against.NullOrEmpty(targetUrl);

        _streamSender.PrepareTarget($"{targetUrl}/start");
        _streamReceiver.ConnectStreamBySource(source);

        _streamReceiver.OnConnectionEstablished += () =>
        {
            _streamSender.SendStreamToEndpoint(_streamReceiver, $"{targetUrl}/inference");
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

    public void Dispose()
    {
        _streamReceiver.Dispose();
        _streamSender.Dispose();
    }
}
