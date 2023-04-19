using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using k8s;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Services;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Data.Repositories;
using RealtimeCv.Infrastructure.Http;
using RealtimeCv.Infrastructure.Messaging;
using RealtimeCv.Infrastructure.Streaming;

namespace RealtimeCv.Infrastructure.Extensions;

public static class ServiceCollectionSetupExtensions
{
    public static void AddDbContext(this IServiceCollection services, string? connectionString) =>
      services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(connectionString)
      );

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IVisionSetRepository, VisionSetRepository>();
    }

    public static void AddStreamHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IStreamReceiver, StreamReceiver>();
        services.AddSingleton<IStreamSender, StreamSender>();
        services.AddTransient<IStreamService, StreamService>();
    }

    public static void AddKubernetes(this IServiceCollection services)
    {
        // TODO: replace with AKS API access
        // Enable proxy with "kubectl proxy --port=8080 &"
        
        services.AddTransient(typeof(Kubernetes), _ => 
            new Kubernetes(new KubernetesClientConfiguration
            {
                Host = "http://localhost:8080/"
            })
        );
    }

    public static void AddConnectionServices(this IServiceCollection services)
    {
        services.AddTransient<IHttpService, HttpService>();
    }

    public static void ConfigureJson(this IServiceCollection services)
    {
        services.Configure<JsonSerializerSettings>(options =>
        {
            options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });

        // services.Configure<JsonSerializerOptions>(options =>
        // {
        //     options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        //     options.WriteIndented = true;
        // });
    }
}
