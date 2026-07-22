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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(OrderFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}