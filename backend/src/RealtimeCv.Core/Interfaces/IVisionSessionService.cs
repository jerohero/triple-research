using System.Threading.Tasks;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Interfaces;

public interface IVisionSessionService
{
    Task<Session> Start(int sessionId);

    public Task Stop(int sessionId);
    
    void Dispose();
}
