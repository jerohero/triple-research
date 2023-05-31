using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class SessionsByVisionSet
      : Specification<Session>
  {
    public SessionsByVisionSet(int visionSetId)
    {
        Query.Where(s => s.VisionSetId == visionSetId)
            .EnableCache(nameof(SessionsByVisionSet), visionSetId);
    }
  }
}
