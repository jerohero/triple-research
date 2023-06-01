using System.Linq;
using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class TrainedModelBySessionSpec
      : Specification<TrainedModel>, ISingleResultSpecification<TrainedModel>
  {
    public TrainedModelBySessionSpec(int sessionId)
    {
        Query.Where(tm => tm.VisionSets.Any(vs => vs.Sessions.Any(s => s.Id == sessionId)))
            .Take(1)
            .EnableCache(nameof(TrainedModelBySessionSpec), sessionId);
    }
  }
}
