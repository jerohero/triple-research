using System.Threading.Tasks;

namespace RealtimeCv.Infrastructure.Interfaces;

public interface IPubSub
{
    Task Send(object message);
}
