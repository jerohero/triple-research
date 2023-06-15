using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO.MemoryMappedFiles;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using OpenCvSharp;

namespace RealtimeCv.Research; 

public class StreamSender {
    private readonly ILogger<StreamSender> _logger;
    private Thread? _sendThread;
    private Thread? _prepareThread;
    private bool _isPrepared;

    public StreamSender(
        ILogger<StreamSender> logger)
    {
        _logger = logger;
    }

    public void PrepareTarget()
    {
        if (_prepareThread is { IsAlive: true })
        {
            _logger.LogInformation("Prepare thread is already running.");
            return;
        }
        
        _prepareThread = new Thread(PrepareEndpoint)
        {
            IsBackground = true
        };

        _prepareThread.Start();
    }
    
    private async void PrepareEndpoint()
    {
        bool didPrepare = false;
        
        // Executes in a loop because the target may not have started yet
        while (!didPrepare)
        {
            try
            {
                await Post("http://localhost:5000/start");
                
                _logger.LogInformation("Prepared target");

                didPrepare = true;
            }
            catch (HttpRequestException)
            {
                didPrepare = false;
                
                _logger.LogInformation("Failed to prepare target. Retrying in 5 seconds");
                
                Thread.Sleep(5000);
            }
        }

        _isPrepared = didPrepare;
    }
    
    public void SendStreamToEndpoint()
    {
        // _sendThread = new Thread(SendFramesToTargetHttp)
        _sendThread = new Thread(SendFramesToTargetSharedMemory)
        // _sendThread = new Thread(SendFramesToTcpSocket)
        {
            IsBackground = true
        };

        _sendThread.Start();
    }

    private async void SendFramesToTargetHttp()
    {
        VideoCapture cap = new("rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5");
        Mat frame = new();
        cap.Read(frame);

        List<double> elapsedSecondList = new();

        while (cap.IsOpened())
        {
            if (elapsedSecondList.Count >= 300) {
                break;
            }
            
            cap.Grab();
            cap.Retrieve(frame);

            byte[] jpg = await FrameToJpg(frame);
            
            if (frame.Empty())
            {
                break;
            }

            try
            {
                DateTime now = DateTime.UtcNow;
                
                using HttpClient client = new();

                using MultipartFormDataContent content = new(
                    "Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)
                )
                {
                    { new ByteArrayContent(jpg), "file", "upload.jpg" }
                };

                // HttpResponseMessage res = await client.PostAsync("http://127.0.0.1:5000/ipc-http", content);
                HttpResponseMessage res = await client.PostAsync("http://127.0.0.1:8000/ipc-http-fastapi", content);
                // object? results = await res.Content.ReadFromJsonAsync<object>();

                double time = (DateTime.UtcNow - now).TotalSeconds;

                _logger.LogInformation($"Sent frame in {time} seconds");
                
                elapsedSecondList.Add(time);
            }
            catch (HttpRequestException)
            {
            }
        }
        
        ToCsv(elapsedSecondList);
    }
    
    private async void SendFramesToTargetSharedMemory()
    {
        VideoCapture cap = new("rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5");
        
        Mat frame = new();
        cap.Read(frame);

        // Image frame = await Image.LoadAsync("test_frame.jpg");
        // using MemoryStream ms = new();
        // await frame.SaveAsPngAsync(ms);
        // byte[] imageBytes = ms.ToArray();
        
        string imageSharedMemoryName = "image";
        string imageSizeSharedMemoryName = "imageSize";
        string resultSharedMemoryName = "result";
        string imageSemaphoreName = "imageReady";
        string resultSemaphoreName = "resultReady";
        int sharedMemorySize = 1024 * 5; // 5 KB
    
        // Create semaphores for signaling
        EventWaitHandle imageSemaphore = new(false, EventResetMode.AutoReset, imageSemaphoreName);
        EventWaitHandle resultSemaphore = new(false, EventResetMode.AutoReset, resultSemaphoreName);
        
        MemoryMappedFile imageMmf = null;
        MemoryMappedFile imageSizeMmf = null;
        MemoryMappedFile resultMmf = null;
        MemoryMappedViewAccessor imageAccessor = null;
        MemoryMappedViewAccessor imageSizeAccessor = null;
        MemoryMappedViewAccessor resultAccessor = null;
        
        List<double> elapsedSecondList = new();

        // Continuously write images to shared memory and read results from shared memory
        while (cap.IsOpened())
        {
            if (elapsedSecondList.Count >= 300) {
                break;
            }
            
            cap.Grab();
            cap.Retrieve(frame);
            
            if (frame.Empty())
            {
                break;
            }

            byte[] frameBuffer = await FrameToJpg(frame);
            
            DateTime start = DateTime.UtcNow;

            int imageSize = frame.Size().Height * frame.Size().Width * 3;
            
            if (imageMmf is null) {
                imageMmf = MemoryMappedFile.CreateOrOpen(imageSharedMemoryName, imageSize);
                imageAccessor = imageMmf.CreateViewAccessor();
            }

            // w/ size
            // if (imageSizeMmf is null)
            // {
            //     imageSizeMmf = MemoryMappedFile.CreateOrOpen(imageSizeSharedMemoryName, sizeof(int));
            //     imageSizeAccessor = imageSizeMmf.CreateViewAccessor();
            // }
            
            // imageSizeAccessor.Write(0, imageSize); // w/ size
            imageAccessor.WriteArray(0, frameBuffer, 0, frameBuffer.Length);

            // Signal that new image data is available
            imageSemaphore.Set();
            
            // Wait for result data to be available
            resultSemaphore.WaitOne();
            
            if (resultMmf is null)
            {
                resultMmf = MemoryMappedFile.CreateOrOpen(resultSharedMemoryName, sizeof(int));
                resultAccessor = resultMmf.CreateViewAccessor();
            }
            
            byte[] buffer = new byte[sharedMemorySize];
            resultAccessor.ReadArray(0, buffer, 0, buffer.Length);

            string result = Encoding.UTF8.GetString(buffer);
            // Console.WriteLine("Result: " + result);;

            TimeSpan elapsed = DateTime.UtcNow - start;
            
            elapsedSecondList.Add(elapsed.TotalSeconds);
            Console.WriteLine($"Writing Duration: {elapsed.TotalSeconds} s");
        }
        
        cap.Dispose();
        imageMmf?.Dispose();
        
        ToCsv(elapsedSecondList);
    }

    private async void SendFramesToTcpSocket() {
        VideoCapture cap = new("rtmp://live.restream.io/live/re_6435068_ac960121c66cd1e6a9f5");
        Mat frame = new();
        cap.Read(frame);

        int port = 5000;
        string server = "localhost";
        TcpClient client = new(server, port);
        NetworkStream stream = client.GetStream();
        
        List<double> elapsedSecondList = new();

        try
        {
            while (cap.IsOpened())
            {
                if (elapsedSecondList.Count >= 300) {
                    break;
                }
                
                cap.Grab();
                cap.Retrieve(frame);

                if (frame.Empty())
                {
                    break;
                }
                
                byte[] frameBuffer = await FrameToJpg(frame);

                DateTime start = DateTime.UtcNow;
                
                // Write frame size to TCP stream
                int frameSize = frameBuffer.Length;
                byte[] frameSizeBytes = BitConverter.GetBytes(frameSize);
                stream.Write(frameSizeBytes, 0, frameSizeBytes.Length);

                // Write frame data to TCP stream
                stream.Write(frameBuffer, 0, frameBuffer.Length);

                // Wait for response
                byte[] resultBuffer = new byte[1024];
                int bytesRead = stream.Read(resultBuffer, 0, resultBuffer.Length);
                string result = Encoding.UTF8.GetString(resultBuffer, 0, bytesRead);
                // Console.WriteLine("Result: " + result);
                
                var elapsed = DateTime.UtcNow - start;
            
                elapsedSecondList.Add(elapsed.TotalSeconds);
                _logger.LogInformation($"Writing Duration: {elapsed.TotalSeconds} s");
            }
        }
        catch (Exception e)
        {
            _logger.LogInformation("Error: " + e.Message);
        }
        
        stream.Close();
        client.Close();
        
        ToCsv(elapsedSecondList);
    }

    private async Task Post(string url)
    {
        using HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.PostAsync(url, null);

        string responseString = await response.Content.ReadAsStringAsync();
    }
    
    private void ToCsv(IReadOnlyList<double> data) {
        const string csvFilePath = "results.csv";

        using StreamWriter writer = new(csvFilePath);
        
        writer.WriteLine("Execution;Elapsed");
        for (int i = 0; i < data.Count; i++)
        {
            writer.WriteLine($"{i + 1};{data[i]}");
        }
    }
    
    private async Task<byte[]> FrameToJpg(Mat frame) {
        using MemoryStream ms = new();

        Image img = Image.Load<Rgba32>(frame.ToBytes());
        await img.SaveAsJpegAsync(ms);
        
        return ms.ToArray();
    }
}