using System;
using k8s;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Services;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Data.Repositories;
using RealtimeCv.Infrastructure.Http;
using RealtimeCv.Infrastructure.Kubernetes;
using RealtimeCv.Infrastructure.Messaging;
using RealtimeCv.Infrastructure.Streaming;

namespace RealtimeCv.Infrastructure.Extensions;

public static class ServiceCollectionSetupExtensions
{
    public static void AddDbContext(this IServiceCollection services, string? connectionString) =>
        services.AddDbContext<AppDbContext>(dbContextOptions =>
            dbContextOptions.UseSqlServer(connectionString, sqlServerOptions => 
                sqlServerOptions.EnableRetryOnFailure()
            )
        );

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IVisionSetRepository, VisionSetRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();
        services.AddScoped<ITrainedModelRepository, TrainedModelRepository>();
    }

    public static void AddStreamHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IStreamReceiver, StreamReceiver>();
        services.AddSingleton<IStreamSender, StreamSender>();
    }

    public static void AddKubernetes(this IServiceCollection services)
    {
        // TODO: replace with AKS API access
        // Enable proxy with "kubectl proxy --port=8080 &"
        services.AddSingleton<IKubernetesService, KubernetesService>();
        
        services.AddTransient(typeof(k8s.Kubernetes), _ => 
            new k8s.Kubernetes(new KubernetesClientConfiguration
            {
                Host = "http://localhost:8080/"
            })
        );
    }

    public static void AddConnectionServices(this IServiceCollection services)
    {
        services.AddHttpClient<IHttpService, HttpService>()
            .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
            {
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)
            }));
    }
    
    public static void AddAsynchronousMessagingServices(this IServiceCollection services)
    {
        services.AddSingleton<IPubSub, PubSub>();
    }
    
    public static void AddQueueMessagingServices(this IServiceCollection services)
    {
        services.AddSingleton<IQueue, Queue>();
    }
    
    public static void AddBlobServices(this IServiceCollection services)
    {
        services.AddSingleton<IBlob, Blob.Blob>();
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
