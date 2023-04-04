using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IQueue
{
    public Task SendMessage(string queueName, object message);
}
