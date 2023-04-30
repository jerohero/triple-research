using System;
using System.Threading.Tasks;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamService
{
    event Action OnStreamEnded;
    
    void HandleStream(string source, string targetUrl, string targetHub);
    
    void Dispose();
}
