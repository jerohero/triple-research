using System;
using System.Threading.Tasks;
using Emgu.CV;
using JetBrains.Annotations;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamReceiver
{
  Mat Frame { get; }
  event Action OnConnectionEstablished;
  event Action OnConnectionBroken;
  
  void ConnectStreamBySource(string source);
  
  void Dispose();
}
