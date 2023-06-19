using System.Threading.Tasks;
using k8s.Models;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Interfaces;

public interface IKubernetesService
{
    public Task<V1Pod> CreateSessionPod(Session session);
    public Task<V1Pod> DeletePod(string podName);
    public Task<V1PodList> GetVisionSetPods(string projectName, string visionSetName);
    Task<V1Pod> GetSessionPod(string podName);
}
