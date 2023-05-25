﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using RealtimeCv.Core.Interfaces;
using RealtimeCv.Core.Services;
using RealtimeCv.Infrastructure.Data;
using RealtimeCv.Infrastructure.Data.Repositories;
using RealtimeCv.Infrastructure.Http;
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

    public static void ConfigureJson(this IServiceCollection services)
    {
        services.Configure<JsonSerializerSettings>(options =>
        {
            options.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        });
    }
}
