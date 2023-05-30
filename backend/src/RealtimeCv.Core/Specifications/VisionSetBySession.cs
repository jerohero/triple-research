using System.Linq;
using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class VisionSetBySession
      : Specification<VisionSet>, ISingleResultSpecification<VisionSet>
  {
    public VisionSetBySession(int sessionId)
    {
        Query.Where(vs => vs.Sessions.Any(s => s.Id == sessionId))
            .Take(1)
            .EnableCache(nameof(VisionSetBySession), sessionId);
    }
  }
}
