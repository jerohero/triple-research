using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IPubSub
{
  Task Init();
  Task Send(object message);
}
