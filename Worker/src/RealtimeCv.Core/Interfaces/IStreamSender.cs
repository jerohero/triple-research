using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamSender
{
    void SendStreamToEndpoint(IStreamReceiver streamReceiver, string targetUrl, string prepareUrl);

    void Dispose();
}
