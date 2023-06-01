using System.Collections.Generic;

namespace RealtimeCv.Core.Models;

public class StreamPollChunkMessage
{
    public int VisionSetId { get; set; }
    public List<string> Sources { get; set; }
}
