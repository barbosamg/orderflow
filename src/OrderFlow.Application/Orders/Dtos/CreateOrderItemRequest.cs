namespace OrderFlow.Application.Orders.Dtos;

public sealed record CreateOrderItemRequest(
    Guid ProductId,
    int Quantity);