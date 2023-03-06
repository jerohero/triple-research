using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamSender
{
  void SendStreamToEndpoint(IStreamReceiver streamReceiver, string url);
  
  void Dispose();
}
