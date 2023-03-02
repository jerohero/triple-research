using System.Threading.Tasks;
using RealtimeCv.Core.Entities;

namespace RealtimeCv.Core.Interfaces;

public interface IUrlStatusChecker
{
  Task<UrlStatusHistory> CheckUrlAsync(string url, string requestId);
}
