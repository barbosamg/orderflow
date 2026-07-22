using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderFlow.Application.Common.Contracts;
using OrderFlow.Application.Customers.Repositories;
using OrderFlow.Application.Products.Repositories;
using OrderFlow.Infrastructure.Customers.Repositories;
using OrderFlow.Infrastructure.Persistence;
using OrderFlow.Infrastructure.Products.Repositories;

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

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<IUnitOfWork>(
            serviceProvider =>
                serviceProvider.GetRequiredService<OrderFlowDbContext>());

        return services;
    }
}