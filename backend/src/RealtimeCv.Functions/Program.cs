
using System.Threading.Tasks;
using k8s;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Services;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Services;
using RealtimeCv.Infrastructure.Extensions;
using RealtimeCv.Infrastructure.Messaging;
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
            services.AddSingleton(typeof(ILoggerAdapter<>), typeof(LoggerAdapter<>));

            services.AddDbContext(hostContext.Configuration.GetValue<string>("SqlConnectionString"));
            services.AddRepositories();
            services.AddKubernetes();
            services.AddSingleton<IPubSub, PubSub>();

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IVisionSetService, VisionSetService>();
            services.AddScoped<ISessionService, SessionService>();
            
            services.AddSingleton<IStreamReceiver, StreamReceiver>();
            
            services.AddConnectionServices();
            services.AddQueueMessagingServices();

            services.AddStreamPollHandlers();

            services.AddAutoMapper(typeof(AutomapperMaps));
        });
}
