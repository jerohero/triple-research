using System.Collections;
using System.Drawing;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace CvWorker;

public class Streams : IDisposable {
    private Thread[] _threads;
    public Mat[] Frames;
    private VideoCapture[] _captures;
    private int _imgSize;
    private int _fps;

    public Streams(IReadOnlyList<string> sources, int imgSize=640, int stride=32) {
        _imgSize = imgSize;
        int n = sources.Count;
        Frames = new Mat[n];
        _threads = new Thread[n];
        _captures = new VideoCapture[n];
        
        for (int i = 0; i < n; i++) {
            int srcIndex = i;
            
            _captures[srcIndex] = new VideoCapture(sources[srcIndex]);
        
            if (!_captures[srcIndex].IsOpened) {
                Console.WriteLine($"Failed to open { sources[srcIndex] }");
                return;
            }

            int w = (int)_captures[srcIndex].Get(CapProp.FrameWidth);
            int h = (int)_captures[srcIndex].Get(CapProp.FrameHeight);
            _fps = (int)_captures[srcIndex].Get(CapProp.Fps);

            Frames[srcIndex] = new Mat();
            _captures[srcIndex].Read(Frames[srcIndex]); // guarantee first frame

            _threads[srcIndex] = new Thread(() => Update(srcIndex, _captures[srcIndex])) {
                IsBackground = true
            };
        
            Console.WriteLine($"Success { w }x{ h } at { _fps } FPS");
        
            _threads[srcIndex].Start();   
        }
    }

    private void Update(int i, VideoCapture capture)
    {
        // Read next stream frame in a daemon thread
        int n = 0;
        
        while (capture.IsOpened) {
            n += 1;
            capture.Grab();
            
            if (n == 4) { // read every 4th frame
                Mat frame = new();
                bool success = capture.Retrieve(frame);
                Frames[i] = success ? frame : new Mat(_imgSize, _imgSize, DepthType.Cv8U, 3);
                n = 0;
            }
            
            Thread.Sleep(1000 / _fps); // wait time
        }
    }

    public void Dispose() {
        foreach (Thread thread in _threads) {
            thread.Join();
        }
        foreach (Mat frame in Frames) {
            frame.Dispose();
        }
    }
}