using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IQueueReceiver
{
  Task<string> GetMessageFromQueue(string queueName);
}
