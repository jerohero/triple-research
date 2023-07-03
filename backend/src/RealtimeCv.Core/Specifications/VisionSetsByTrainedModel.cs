using Ardalis.Specification;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications
{
  public class VisionSetsByTrainedModel
      : Specification<VisionSet>
  {
    public VisionSetsByTrainedModel(int trainedModelId)
    {
        Query.Where(vs => vs.TrainedModelId == trainedModelId)
            .EnableCache(nameof(VisionSetsByTrainedModel), trainedModelId);
    }
  }
}
