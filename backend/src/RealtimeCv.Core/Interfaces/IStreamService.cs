using System;
using System.Threading.Tasks;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamService
{
    event Action OnStreamEnded;
    
    void HandleStream(Session session, string targetUrl);
    
    void Dispose();
}
