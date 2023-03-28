using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IQueueSender
{
    Task SendMessageToQueue(string message, string queueName);
}
