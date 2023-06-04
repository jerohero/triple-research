using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Azure.Identity;
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

            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            services.AddDbContext(hostContext.Configuration.GetValue<string>("SqlConnectionString"));
            services.AddRepositories();
            services.AddKubernetes();
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
