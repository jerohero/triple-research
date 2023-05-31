﻿using Ardalis.GuardClauses;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Core.Worker.Services;

/// <summary>
/// Service that oversees the process of consuming the input, sending it to the inference API and publishing the results.
/// </summary>
public class StreamService : IStreamService, IDisposable
{
    public event Action? OnStreamEnded;
    private readonly IStreamReceiver _streamReceiver;
    private readonly IStreamSender _streamSender;
    private readonly IPubSub _pubSub;
    private readonly IBlob _blob;

    public StreamService(
      IStreamReceiver streamReceiver,
      IStreamSender streamSender,
      IPubSub pubSub,
      IBlob blob)
    {
        _streamReceiver = streamReceiver;
        _streamSender = streamSender;
        _pubSub = pubSub;
        _blob = blob;
    }

    public void HandleStream(Session session, string modelUri, string targetUrl)
    {
        Guard.Against.NullOrWhiteSpace(session.Source, nameof(session.Source));
        Guard.Against.NullOrEmpty(targetUrl);

        _streamSender.PrepareTarget($"{targetUrl}/start", modelUri);
        _streamReceiver.ConnectStreamBySource(session.Source);

        _streamReceiver.OnConnectionEstablished += () =>
        {
            _streamSender.SendStreamToEndpoint(_streamReceiver, $"{targetUrl}/inference");
        };

        _streamReceiver.OnConnectionTimeout += () =>
        {
            OnStreamEnded?.Invoke();
            _streamReceiver.Dispose();
            _streamSender.Dispose();
            // TODO terminate pod
        };

        _streamSender.OnPredictionResult += async result =>
        {
            await _pubSub.Send(result, session.Pod, "predictions");
            
            // Store in db
        };

        _streamSender.OnConnectionTimeout += () =>
        {
            _streamReceiver.Dispose();
            _streamSender.Dispose();
            // TODO terminate pod
        };
    }
    
    public void Dispose()
    {
        _streamReceiver.Dispose();
        _streamSender.Dispose();
    }
}