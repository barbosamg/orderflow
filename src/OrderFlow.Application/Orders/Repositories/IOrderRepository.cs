using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Order?> GetByIdForUpdateAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task AddAsync(
        Order order,
        CancellationToken cancellationToken);
}