using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var config = new ConfigurationBuilder()
    .AddJsonFile("local.settings.json", optional: false, reloadOnChange: true)
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
