namespace OrderFlow.Application.Orders.Dtos;

public sealed record OrderItemResponse(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal);