using System.Threading.Tasks;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Interfaces;

public interface ISessionHandlerService
{
    Task<Session> SetSessionActive(int sessionId);

    public Task EndSession(int sessionId);
    
    void Dispose();
}
