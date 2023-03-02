using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Http;
using RealtimeCv.Infrastructure.Messaging;

namespace RealtimeCv.Infrastructure;

public static class ServiceCollectionSetupExtensions
{
  public static void AddDbContext(this IServiceCollection services, IConfiguration configuration) =>
      services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(
              configuration.GetConnectionString("DefaultConnection")));

  public static void AddRepositories(this IServiceCollection services) =>
      services.AddScoped<IRepository, EfRepository>();

  public static void AddMessageQueues(this IServiceCollection services)
  {
    services.AddSingleton<IQueueReceiver, InMemoryQueueReceiver>();
    services.AddSingleton<IQueueSender, InMemoryQueueSender>();
  }

  public static void AddUrlCheckingServices(this IServiceCollection services)
  {
    services.AddTransient<IUrlStatusChecker, UrlStatusChecker>();
    services.AddTransient<IHttpService, HttpService>();
  }
}
