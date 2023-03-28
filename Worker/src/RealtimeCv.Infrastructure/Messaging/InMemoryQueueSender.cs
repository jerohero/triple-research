using System.Threading.Tasks;
using RealtimeCv.Core.Interfaces;

namespace RealtimeCv.Infrastructure.Messaging;

/// <summary>
/// A simple implementation using the built-in Queue type
/// </summary>
public class InMemoryQueueSender : IQueueSender
{
    public async Task SendMessageToQueue(string message, string queueName)
    {
        await Task.CompletedTask; // just so async is allowed
        InMemoryQueueReceiver.MessageQueue.Enqueue(message);
    }
}
