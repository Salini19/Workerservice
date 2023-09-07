using Workerservice;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options => { options.ServiceName = "My Worker Service"; })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
