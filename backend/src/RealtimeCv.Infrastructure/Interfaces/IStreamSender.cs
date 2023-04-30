using System;
using JetBrains.Annotations;

namespace RealtimeCv.Infrastructure.Interfaces;

public interface IStreamSender
{
    [CanBeNull] event Action<object?> OnPredictionResult;
    event Action OnConnectionTimeout;
    
    void SendStreamToEndpoint(IStreamReceiver streamReceiver, string targetUrl);
    
    void PrepareTarget(string? prepareUrl, int secondsBeforeTimeout = 180);
    
    void Dispose();

}
