using Emgu.CV;
using Emgu.CV.CvEnum;

namespace CvWorker;

public class StreamConnection : IDisposable {
    public Mat Frame;
    private readonly string _source;
    private int _fps;
    private VideoCapture? _capture;
    private Thread? _updateThread;
    private readonly Thread? _pollThread;
    public event ConnectionEventHandler OnConnectionEstablished;
    public event ConnectionEventHandler OnConnectionBroken;

    public delegate void ConnectionEventHandler();

    public StreamConnection(string source, int imgSize=640, int stride=32) {
        _source = source;
        Frame = new Mat();

        _pollThread = new Thread(PollStream) {
            IsBackground = true
        };
        
        _pollThread.Start();

        OnConnectionEstablished += ReadStream;
        OnConnectionBroken += PollStream;
    }

    private void PollStream() {
        VideoCapture capture = new(_source);

        while (!capture.IsOpened) {
            Console.WriteLine($"Failed to open { _source }. Retrying in 10 sec..");
            Thread.Sleep(1000 * 10);
            capture = new VideoCapture(_source);
        }

        capture.Set(CapProp.Buffersize, 2);
        _capture = capture;
        
        OnConnectionEstablished();
    }

    private void ReadStream() {
        // int w = (int)_capture.Get(CapProp.FrameWidth);
        // int h = (int)_capture.Get(CapProp.FrameHeight);
        if (!_capture!.IsOpened) {
            Console.WriteLine("Stream is inactive");
            return;
        }
        
        _fps = (int)_capture.Get(CapProp.Fps);
        // Console.WriteLine($"Success { w }x{ h } at { _fps } FPS");
        _capture.Read(Frame); // guarantee first frame

        _updateThread = new Thread(Update) {
            IsBackground = true
        };
        
        _updateThread.Start();
    }

    private void Update()
    {
        // Read next stream frame in a daemon thread
        while (_capture!.IsOpened) {
            _capture.Grab();
            
            Mat frame = new();
            _capture.Retrieve(frame);
            Frame = frame;

            Thread.Sleep(1000 / _fps); // wait time
        }

        OnConnectionBroken();
    }

    public void Dispose() {
        _updateThread?.Join();
        _pollThread?.Join();
        _capture?.Dispose();
        Frame.Dispose();
    }
}