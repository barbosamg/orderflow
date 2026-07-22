using OrderFlow.Application.Orders.Dtos;

namespace OrderFlow.Application.Orders.Contracts;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken);

    Task<OrderResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<OrderResponse?> ChangeStatusAsync(
        Guid id,
        ChangeOrderStatusRequest request,
        CancellationToken cancellationToken);
}