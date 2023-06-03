using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Services;
using RealtimeCv.Core.Worker.Services;
using RealtimeCv.Infrastructure.Extensions;

namespace RealtimeCv.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
                services.AddSingleton<IEntryPointService, EntryPointService>();
                services.AddSingleton<IServiceLocator, ServiceScopeFactoryLocator>();
                
                services.AddStreamHandlers();
                services.AddAsynchronousMessagingServices();
                services.AddBlobServices();

                services.AddTransient<IStreamService, StreamService>();
                services.AddSingleton<ISessionHandlerService, SessionHandlerHandlerService>();

                services.AddDbContext(Environment.GetEnvironmentVariable("SqlConnectionString"));
                services.AddRepositories();
                services.AddConnectionServices();

                services.ConfigureJson();

                services.Configure<WorkerSettings>(hostContext.Configuration.GetSection("WorkerSettings"));

                services.AddHostedService<Worker>();
            });
}
