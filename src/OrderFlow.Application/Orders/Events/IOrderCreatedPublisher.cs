namespace OrderFlow.Application.Orders.Events;

public interface IOrderCreatedPublisher
{
    Task PublishAsync(
        OrderCreatedEvent orderCreatedEvent,
        CancellationToken cancellationToken);
}