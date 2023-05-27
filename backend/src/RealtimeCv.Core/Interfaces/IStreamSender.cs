using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamSender
{
    [CanBeNull] event Action<object> OnPredictionResult;
    event Action OnConnectionTimeout;
    
    void SendStreamToEndpoint(IStreamReceiver streamReceiver, string targetUrl);
    
    void PrepareTarget(string prepareUrl, string datasetUri, int secondsBeforeTimeout = 180);
    
    void Dispose();

}
