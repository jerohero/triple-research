using System.Threading.Tasks;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamReceiver
{
  Task<string> GetStreamFromSource(string source);
}
