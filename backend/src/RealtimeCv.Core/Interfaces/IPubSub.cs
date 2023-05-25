using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IPubSub
{
    Task Send(object message);
}
