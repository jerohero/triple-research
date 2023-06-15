using RealtimeCv.Research;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services => {
        services.AddHostedService<Worker>();
        services.AddSingleton<StreamReceiver>();
        services.AddSingleton<StreamSender>();
    })
    .Build();

host.Run();