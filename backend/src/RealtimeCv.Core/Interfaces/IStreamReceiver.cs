using System;
using OpenCvSharp;

namespace RealtimeCv.Core.Interfaces;

public interface IStreamReceiver
{
    Mat Frame { get; }
    event Action OnConnectionEstablished;
    event Action OnConnectionBroken;
    event Action OnConnectionTimeout;

    void ConnectStreamBySource(string source, int secondsBeforeTimeout = 15);

    bool CheckConnection(string source);

    void Dispose();
}
