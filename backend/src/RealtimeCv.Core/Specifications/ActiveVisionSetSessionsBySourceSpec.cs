using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using k8s.Models;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Specifications;

public class ActiveVisionSetSessionsBySourceSpec
    : Specification<Session>
{
    public ActiveVisionSetSessionsBySourceSpec(int visionSetId, string source, IList<V1Pod> pods)
    {
        var activePodNames = new HashSet<string>(
            pods
                .Where(pod => pod.Status.Phase != "Unknown")
                .Select(pod => pod.Metadata.Name)
        );

        Query.Where(p =>
                p.Source == source
                && p.VisionSetId == visionSetId
                && activePodNames.Contains(p.Pod)
            )
            .EnableCache(nameof(ActiveVisionSetSessionsBySourceSpec), visionSetId, source, pods);
    }
}
