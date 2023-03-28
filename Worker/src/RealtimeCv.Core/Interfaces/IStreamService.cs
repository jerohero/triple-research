using System.Threading.Tasks;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamService
{
    void HandleStream(string source, string targetUrl, string prepareUrl);
}
