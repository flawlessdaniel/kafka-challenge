using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using statusUpdatedService.Services.Abstractions;
using statusUpdatedService.Services.Common;
using statusUpdatedService.Services.Implementations;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(cnf => {
        cnf.SetBasePath(Directory.GetCurrentDirectory());
        cnf.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices(cnf => {
        cnf.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect("localhost:6379"));
        var services = cnf.BuildServiceProvider();
        var configurations = services.GetRequiredService<IConfiguration>();
        cnf.AddInfrastructure(configurations);
        cnf.Configure<KafkaSettings>(configurations.GetSection("Kafka"));
        cnf.AddTransient<ICacheService, RedisService>();
        cnf.AddSingleton<IKafkaProducer, KafkaProducer>();
        cnf.AddHostedService<KafkaConsumer>();
    })
    .ConfigureLogging(cnf => {
        cnf.AddConsole();
    })
    .Build();

await host.RunAsync();