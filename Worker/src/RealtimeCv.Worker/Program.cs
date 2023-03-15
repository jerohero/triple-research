using Microsoft.Extensions.Azure;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Services;
using RealtimeCv.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using RealtimeCv.Core.Settings;
using RealtimeCv.Infrastructure.Messaging;

namespace RealtimeCv.Worker;

public class Program
{
  public static void Main(string[] args)
  {
    var host = CreateHostBuilder(args).Build();

    // seed some queue messages
    var queueSender = (IQueueSender)host.Services.GetRequiredService(typeof(IQueueSender));
    for (int i = 0; i < 10; i++)
    {
      queueSender.SendMessageToQueue("https://google.com", "urlcheck");
    }

    host.Run();
  }

  public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
          .ConfigureServices((hostContext, services) =>
          {
            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));
            services.AddSingleton<IEntryPointService, EntryPointService>();
            services.AddSingleton<IServiceLocator, ServiceScopeFactoryLocator>();

            // Infrastructure.ContainerSetup
            services.AddMessageQueues();
            services.AddStreamHandlers();

            services.AddSingleton<IPubSub, PubSub>();
            
            services.AddDbContext(hostContext.Configuration);
            services.AddRepositories();
            services.AddUrlCheckingServices();

            var workerSettings = new WorkerSettings();
            hostContext.Configuration.Bind(nameof(WorkerSettings), workerSettings);
            services.AddSingleton(workerSettings);

            var entryPointSettings = new EntryPointSettings();
            hostContext.Configuration.Bind(nameof(EntryPointSettings), entryPointSettings);
            services.AddSingleton(entryPointSettings);

            var azureSettings = new AzureSettings();
            hostContext.Configuration.Bind(nameof(AzureSettings), azureSettings);
            services.AddSingleton(azureSettings);

            services.AddHostedService<Worker>();
          });
}
