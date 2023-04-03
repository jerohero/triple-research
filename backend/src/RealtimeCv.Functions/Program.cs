
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Functions.Interfaces;
using RealtimeCv.Functions.Services;
using RealtimeCv.Infrastructure.Extensions;

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

            services.AddScoped<IProjectService, ProjectService>();

            services.AddAutoMapper(typeof(AutomapperMaps));
        });
}
