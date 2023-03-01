using System.Collections;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CvWorker;

public class Stream : IDisposable {
    public Mat Frame;
    private readonly VideoCapture _capture;
    private readonly int _imgSize;
    private readonly int _fps;
    private readonly Thread? _updateThread;

    public Stream(string source, int imgSize=640, int stride=32) {
        _imgSize = imgSize;
        Frame = new Mat();

        _capture = new VideoCapture(source);
        
        if (!_capture.IsOpened) {
            Console.WriteLine($"Failed to open { source }");
            return;
        }

        int w = (int)_capture.Get(CapProp.FrameWidth);
        int h = (int)_capture.Get(CapProp.FrameHeight);
        _fps = (int)_capture.Get(CapProp.Fps);
        
        _capture.Read(Frame); // guarantee first frame

        _updateThread = new Thread(UpdateFrame) {
            IsBackground = true
        };
        
        Console.WriteLine($"Success { w }x{ h } at { _fps } FPS");
        
        _updateThread.Start();
    }

    private void UpdateFrame()
    {
        // Read next stream frame in a daemon thread
        int n = 0;

        while (_capture.IsOpened) {
            Console.WriteLine("frame!");
            
            n += 1;
            _capture.Grab();
            
            if (n == 4) { // read every 4th frame
                Mat frame = new();
                _capture.Retrieve(frame);
                Frame = frame;
                n = 0;
            }

            //Thread.Sleep(1000 / _fps); // wait time
        }
    }

    public void Dispose() {
        _updateThread?.Join();
        _capture.Dispose();
        Frame.Dispose();
    }
}