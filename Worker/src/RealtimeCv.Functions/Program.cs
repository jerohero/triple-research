using System;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Infrastructure;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Extensions;

namespace RealtimeCv.Functions;

class Program
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
      });
}
