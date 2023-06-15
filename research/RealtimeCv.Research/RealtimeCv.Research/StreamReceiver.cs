using System.Diagnostics;
using System.Drawing;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using FFMpegCore;
using FFMpegCore.Arguments;
using FFMpegCore.Enums;
using OpenCvSharp;
using Image = SixLabors.ImageSharp.Image;

namespace RealtimeCv.Research; 

public class StreamReceiver {
    private readonly ILogger<StreamReceiver> _logger;
    private const string RtmpUrl = "rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5";
    private const string HlsUrl = "https://cdn.rtvdrenthe.nl/live/rtvdrenthe/tv/1080p/prog_index.m3u8";
    private VideoCapture _capture;
    

    public StreamReceiver(
        ILogger<StreamReceiver> logger)
    {
        _logger = logger;
    }

    public void ReadStreamOpenCv()
    {
        _capture = new VideoCapture(RtmpUrl);
        // _capture = new VideoCapture(HlsUrl);

        if (!_capture.IsOpened()) {
            return;
        }

        Thread processFramesOpenCvThread = new(ProcessFramesOpenCv)
        {
            IsBackground = true
        };

        processFramesOpenCvThread.Start();
    }
    
    public void ReadStreamFfmpeg() {
         const int fps = 30;

         if (File.Exists("latest_frame.png")) {
             File.Delete("latest_frame.png");
         }
         
         ProcessStartInfo startInfo = new() {
             FileName = "ffmpeg",
             Arguments = $"-y -an -flv_metadata 1 -analyzeduration 1 -i {RtmpUrl} -update 1 -vf fps={fps} latest_frame.png",
             RedirectStandardOutput = true,
             RedirectStandardError = true,
             UseShellExecute = false,
             CreateNoWindow = true
         };

         Thread ffmpegProcessThread = new(ProcessFramesFfmpeg)
         {
             IsBackground = true
         };
         
         using Process process = new() { StartInfo = startInfo };

         void FfmpegThreadStart() => process.Start();
         Thread ffmpegThread = new(FfmpegThreadStart);
         ffmpegThread.Start();

         ffmpegProcessThread.Start();
    }

    private async void ProcessFramesFfmpeg() {
        List<double> elapsedSecondList = new();
        List<double> totalElapsedList = new();

        DateTime startDate = DateTime.UtcNow;
        
        while (true)
        {
            DateTime now = DateTime.UtcNow;

            try {
                using Image image = await Image.LoadAsync("latest_frame.jpg");

                if (image.Size.IsEmpty) {
                    break;
                }

                TimeSpan duration = DateTime.UtcNow - now;
                TimeSpan totalElapsed = DateTime.UtcNow - startDate;

                elapsedSecondList.Add(duration.TotalSeconds);
                totalElapsedList.Add(totalElapsed.TotalSeconds);

                if (elapsedSecondList.Count >= 300) {
                    break;
                }

                Thread.Sleep((int)TimeSpan.FromSeconds(1 / 30f).TotalMilliseconds);
            }
            catch (FileNotFoundException e) {
            }
        }

        _logger.LogInformation("Connection broken");
        
        ToCsv(elapsedSecondList, totalElapsedList);
    }
    
    public void ReadStreamFfmpegPipe() {
        ProcessStartInfo processStartInfo = new() {
            FileName = "ffmpeg",
            Arguments = $"-i {RtmpUrl} -an -c:v png -f image2pipe -",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        Process process = new();
        process.StartInfo = processStartInfo;

        // Start the process and read the frames from the standard output
        process.Start();
        byte[] buffer = new byte[1024];
        int bytesRead;
        while ((bytesRead = process.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
        {
            // Do something with the frame data here
            // In this example, we just print out the number of bytes read
            Console.WriteLine("{0} bytes read", bytesRead);
            buffer = new byte[1024];
        }

        // Wait for the process to exit
        process.WaitForExit();
    }

    private async void ProcessFramesOpenCv() {
        List<double> elapsedSecondList = new();
        List<double> totalElapsedList = new();
        
        Mat frame = new();
        _capture.Read(frame); // guarantee first frame

        int fps = (int)_capture.Get(VideoCaptureProperties.Fps);
        
        DateTime startDate = DateTime.UtcNow;

        while (_capture.IsOpened())
        {
            DateTime now = DateTime.UtcNow;

            _capture.Grab();

            _capture.Retrieve(frame);

            if (frame.Empty())
            {
                break;
            }
            
            await FrameToPng(frame);
            
            TimeSpan duration = DateTime.UtcNow - now;
            TimeSpan totalElapsed = DateTime.UtcNow - startDate;

            elapsedSecondList.Add(duration.TotalSeconds);
            totalElapsedList.Add(totalElapsed.TotalSeconds);

            if (elapsedSecondList.Count >= 300) {
                break;
            }

            Thread.Sleep((int)TimeSpan.FromSeconds(1f / fps).TotalMilliseconds);
        }

        _logger.LogInformation("Connection broken");
        
        ToCsv(elapsedSecondList, totalElapsedList);
    }

    private async Task FrameToPng(Mat frame) {
        using MemoryStream ms = new();

        Image img = Image.Load<Rgba32>(frame.ToBytes());
        await img.SaveAsJpegAsync(ms);

        await img.SaveAsJpegAsync("testo.jpg");
            
        var pngBits = ms.ToArray();
    }

    private void ToCsv(IReadOnlyList<double> data, IReadOnlyList<double> totalElapsed) {
        const string csvFilePath = "results.csv";

        using StreamWriter writer = new(csvFilePath);
        
        writer.WriteLine("Execution;Elapsed;TotalElapsed");
        for (int i = 0; i < data.Count; i++)
        {
            writer.WriteLine($"{i + 1};{data[i]};{totalElapsed[i]}");
        }
    }
}