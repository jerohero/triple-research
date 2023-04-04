using System.Collections.Generic;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamPollService
{
    List<string> DetectActiveStreams(string[] sources);
    
    void Dispose();
}
