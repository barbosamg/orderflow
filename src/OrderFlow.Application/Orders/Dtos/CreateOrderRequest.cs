namespace OrderFlow.Application.Orders.Dtos;

public sealed record CreateOrderRequest(
    Guid CustomerId,
    IReadOnlyCollection<CreateOrderItemRequest> Items);