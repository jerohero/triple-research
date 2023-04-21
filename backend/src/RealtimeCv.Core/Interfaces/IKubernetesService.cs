using System.Threading.Tasks;
using k8s.Models;

namespace RealtimeCv.Core.Interfaces;

public interface IKubernetesService
{
    public Task<V1Pod> CreateCvPod(int sessionId, string podName);
    public Task<V1Pod> DeletePod(string podName);
}
