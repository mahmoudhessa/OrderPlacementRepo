using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Talabeyah.OrderManagement.Infrastructure;
using Talabeyah.OrderManagement.Worker.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<OrderManagementDbContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
        services.AddHostedService<KafkaAuditConsumer>();
        services.AddHostedService<OrderAutoCancelService>();
    })
    .Build();

await host.RunAsync();
