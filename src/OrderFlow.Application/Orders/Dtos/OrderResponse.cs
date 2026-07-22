using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.Dtos;

public sealed record OrderResponse(
    Guid Id,
    Guid CustomerId,
    IReadOnlyCollection<OrderItemResponse> Items,
    decimal Total,
    OrderStatus Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);