using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtimeCv.Core.Functions.Config;
using RealtimeCv.Core.Functions.Services;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Infrastructure.Extensions;
using RealtimeCv.Infrastructure.Streaming;

namespace RealtimeCv.Functions;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureFunctionsWorkerDefaults()
        .ConfigureServices((hostContext, services) =>
        {
            var isDevelopment = hostContext.HostingEnvironment.IsDevelopment();
            
            Console.WriteLine("IsDevelopment: " + isDevelopment);

            var location = Assembly.GetExecutingAssembly().Location;
            var rootPath = isDevelopment
                ? Path.GetFullPath(Path.Combine(location, "..", "..", "..", ".."))
                : Path.GetDirectoryName(location);
            
            Console.WriteLine("RootPath: " + rootPath);

            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            services.AddDbContext(hostContext.Configuration.GetValue<string>("SqlConnectionString"));
            services.AddRepositories();
            services.AddKubernetes(rootPath!);
            services.AddBlobServices();
            services.AddAsynchronousMessagingServices();

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IVisionSetService, VisionSetService>();
            services.AddScoped<ISessionService, SessionService>();
            services.AddScoped<IStreamDetectionService, StreamDetectionService>();
            
            services.AddSingleton<IStreamReceiver, StreamReceiver>();
            
            services.AddConnectionServices();
            services.AddQueueMessagingServices();

            services.AddAutoMapper(typeof(AutomapperMaps));
        });
}
