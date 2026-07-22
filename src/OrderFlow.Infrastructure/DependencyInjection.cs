using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Common.Contracts;
using OrderFlow.Application.Customers.Repositories;
using OrderFlow.Application.Products.Repositories;
using OrderFlow.Infrastructure.Customers.Repositories;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.Infrastructure.Products.Repositories;
using OrderFlow.Application.Orders.Repositories;
using OrderFlow.Infrastructure.Orders.Repositories;
using OrderFlow.Application.Orders.Events;
using OrderFlow.Infrastructure.Messaging;

namespace OrderFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException(
                "Connection string 'Postgres' was not found.");

        services.AddDbContext<OrderFlowDbContext>(
            options => options.UseNpgsql(connectionString));

        var rabbitMqSection = configuration.GetSection("RabbitMq");

        var rabbitMqOptions = new RabbitMqOptions
        {
            HostName =
                rabbitMqSection["HostName"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ HostName was not found."),

            Port =
                int.TryParse(
                    rabbitMqSection["Port"],
                    out var rabbitMqPort)
                        ? rabbitMqPort
                        : throw new InvalidOperationException(
                            "RabbitMQ Port is invalid."),

            UserName =
                rabbitMqSection["UserName"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ UserName was not found."),

            Password =
                rabbitMqSection["Password"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ Password was not found."),

            VirtualHost =
                rabbitMqSection["VirtualHost"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ VirtualHost was not found."),

            ExchangeName =
                rabbitMqSection["ExchangeName"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ ExchangeName was not found."),

            OrderCreatedRoutingKey =
                rabbitMqSection["OrderCreatedRoutingKey"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ OrderCreatedRoutingKey was not found."),

            OrderCreatedQueue =
                rabbitMqSection["OrderCreatedQueue"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ OrderCreatedQueue was not found."),

            DeadLetterExchangeName =
                rabbitMqSection["DeadLetterExchangeName"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ DeadLetterExchangeName was not found."),

            DeadLetterQueueName =
                rabbitMqSection["DeadLetterQueueName"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ DeadLetterQueueName was not found."),

            PrefetchCount =
                ushort.TryParse(
                    rabbitMqSection["PrefetchCount"],
                    out var prefetchCount)
                        ? prefetchCount
                        : throw new InvalidOperationException(
                            "RabbitMQ PrefetchCount is invalid."),

            ClientProvidedName =
                rabbitMqSection["ClientProvidedName"]
                ?? throw new InvalidOperationException(
                    "RabbitMQ ClientProvidedName was not found."),

            NetworkRecoverySeconds =
                int.TryParse(
                    rabbitMqSection["NetworkRecoverySeconds"],
                    out var networkRecoverySeconds)
                        ? networkRecoverySeconds
                        : throw new InvalidOperationException(
                            "RabbitMQ NetworkRecoverySeconds is invalid."),

            RequestedHeartbeatSeconds =
                int.TryParse(
                    rabbitMqSection["RequestedHeartbeatSeconds"],
                    out var requestedHeartbeatSeconds)
                        ? requestedHeartbeatSeconds
                        : throw new InvalidOperationException(
                            "RabbitMQ RequestedHeartbeatSeconds is invalid.")
        };

        services.AddSingleton(rabbitMqOptions);

        services.AddSingleton<
            IOrderCreatedPublisher,
            RabbitMqOrderCreatedPublisher>();

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();

        services.AddScoped<IUnitOfWork>(
            serviceProvider =>
                serviceProvider.GetRequiredService<OrderFlowDbContext>());

        return services;
    }
}