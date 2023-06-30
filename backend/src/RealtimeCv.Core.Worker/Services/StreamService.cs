using Ardalis.GuardClauses;
using RealtimeCv.Core.Entities;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Models.Dto;

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
    private int _resultCount;

    public StreamService(
        IStreamReceiver streamReceiver,
        IStreamSender streamSender,
        IPubSub pubSub)
    {
        _streamReceiver = streamReceiver;
        _streamSender = streamSender;
        _pubSub = pubSub;
    }

    public void HandleStream(Session session, string modelName, string targetUrl)
    {
        Guard.Against.NullOrWhiteSpace(session.Source, nameof(session.Source));
        Guard.Against.NullOrEmpty(targetUrl);

        _streamSender.PrepareTarget($"{targetUrl}/start", modelName);
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
        };

        _streamSender.OnPredictionResult += async result =>
        {
            _resultCount++;
            
            var message = new PredictionDto
            {
                Index = _resultCount,
                Status = "succeeded",
                CreatedAt = DateTime.UtcNow,
                Result = result
            };
            
            await _pubSub.Send(message, session.Pod, "predictions");
        };

        _streamSender.OnConnectionTimeout += () =>
        {
            _streamReceiver.Dispose();
            _streamSender.Dispose();
        };
    }
    
    public void Dispose()
    {
        _streamReceiver.Dispose();
        _streamSender.Dispose();
    }
}
