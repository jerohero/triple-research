using System;
using OpenCvSharp;

namespace RealtimeCv.Infrastructure.Interfaces;

public interface IStreamReceiver
{
    Mat Frame { get; }
    event Action OnConnectionEstablished;
    event Action OnConnectionBroken;
    event Action OnConnectionTimeout;

    void ConnectStreamBySource(string source, int secondsBeforeTimeout = 15);

    void Dispose();
}
