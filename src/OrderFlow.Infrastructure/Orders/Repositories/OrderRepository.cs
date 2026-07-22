using Microsoft.EntityFrameworkCore;
using OrderFlow.Application.Orders.Repositories;
using OrderFlow.Domain.Orders;
using OrderFlow.Infrastructure.Persistence;

namespace OrderFlow.Infrastructure.Orders.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly OrderFlowDbContext _dbContext;

    public OrderRepository(OrderFlowDbContext dbContext)
    {
        _dbContext = dbContext ??
            throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .SingleOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);
    }

    public async Task<Order?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _dbContext.Orders
            .Include(order => order.Items)
            .SingleOrDefaultAsync(
                order => order.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        Order order,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(order);

        await _dbContext.Orders.AddAsync(
            order,
            cancellationToken);
    }
}