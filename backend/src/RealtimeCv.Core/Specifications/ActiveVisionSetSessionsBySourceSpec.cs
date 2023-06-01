using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class ActiveVisionSetSessionsBySourceSpec
      : Specification<Session>
  {
    public ActiveVisionSetSessionsBySourceSpec(int visionSetId, string source)
    {
        Query.Where(p => 
                p.Source == source
                && p.VisionSetId == visionSetId
                && p.IsActive
            )
            .EnableCache(nameof(ActiveVisionSetSessionsBySourceSpec), visionSetId, source);
    }
  }
}
