using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamSender
{
    [CanBeNull] event Action<object> OnPredictionResult;
    
    void SendStreamToEndpoint(IStreamReceiver streamReceiver, string targetUrl, string prepareUrl);
    
    void PrepareTarget();
    
    void Dispose();

}
