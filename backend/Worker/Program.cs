using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Infrastructure.Persistence;
using Talabeyah.OrderManagement.Worker.Services;
using Talabeyah.OrderManagement.Domain.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
        
        // Register domain services
        services.AddScoped<OrderDomainService>();
        services.AddScoped<ProductDomainService>();
        
        // Register hosted services
        services.AddHostedService<KafkaAuditConsumer>();
        services.AddHostedService<OrderAutoCancelService>();
    })
    .Build();

await host.RunAsync();
