using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamService
{
    void HandleStream(string source, string targetUrl);
    
    void Dispose();
}
