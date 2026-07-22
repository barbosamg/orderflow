using System.Data;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Common.Contracts;
using OrderFlow.Domain.Chats;
using OrderFlow.Domain.Customers;
using OrderFlow.Domain.Events;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Infrastructure.Persistence;

public class OrderFlowDbContext :
    DbContext,
    IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    public DbSet<ProcessedEvent> ProcessedEvents => Set<ProcessedEvent>();

    public OrderFlowDbContext(
        DbContextOptions<OrderFlowDbContext> options)
        : base(options)
    {
    }

    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(operation);

        await using var transaction =
            await Database.BeginTransactionAsync(
                IsolationLevel.Serializable,
                cancellationToken);

        try
        {
            var result = await operation(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);

            throw;
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrderFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}