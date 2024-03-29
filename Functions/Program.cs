using Abstractions;
using Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddSingleton(config.GetSection("BaselinkerSettings").Get<BaselinkerSettings>()!);
        services.AddSingleton(config.GetSection("FaireSettings").Get<FaireSettings>()!);
        services.AddSingleton(config.GetSection("DefaultValues").Get<DefaultValues>()!);
        services.AddSingleton<IStorageService, XmlService>();
        services.AddScoped<IBaselinkerService, BaselinkerService>();
        services.AddScoped<IFaireService, FaireService>();
    })
    .Build();

host.Run();
