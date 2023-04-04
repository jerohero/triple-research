using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamPollService
{
    List<string> DetectActiveStreams(List<string> sources);

    Task SendStreamPollChunkToQueue(List<string> sources);
    
    void Dispose();
}
