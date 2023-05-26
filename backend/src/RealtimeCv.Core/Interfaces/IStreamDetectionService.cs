using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamDetectionService
{
    List<string> DetectActiveStreams(List<string> sources);

    Task SendStreamPollChunkToQueue(List<string> sources);
    
    void Dispose();
}
