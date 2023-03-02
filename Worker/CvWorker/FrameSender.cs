using Emgu.CV;

namespace CvWorker;

public class FrameSender : IDisposable {
    private readonly StreamConnection _streamConnection;
    private Thread? _sendThread;
    private string? _targetUrl;

    public FrameSender(StreamConnection streamConnection) {
        _streamConnection = streamConnection;
    }

    public void SendFrames(string url) {
        _targetUrl = url;
        
        _sendThread = new Thread(SendFramesToTarget) {
            IsBackground = true
        };
        
        _sendThread.Start();
    }

    private void SendFramesToTarget() {
        if (_targetUrl is null) {
            // Handle
            return;
        }

        while (!_streamConnection.Frame.IsEmpty) {
            CvInvoke.Imshow("frames", _streamConnection.Frame);
            CvInvoke.WaitKey(1/30);
            
            // Send frame
        }
    }

    public void Dispose() {
        _sendThread?.Join();
    }
}