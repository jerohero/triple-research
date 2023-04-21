using System.Threading;
using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IEntryPointService
{
    Task Execute(int sessionId, CancellationToken stoppingToken);

    void Stop();
}
