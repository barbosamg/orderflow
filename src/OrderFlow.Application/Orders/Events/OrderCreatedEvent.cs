namespace OrderFlow.Application.Orders.Events;

public sealed record OrderCreatedEvent(
    Guid EventId,
    Guid OrderId,
    Guid CustomerId,
    decimal Total,
    DateTime CreatedAt);