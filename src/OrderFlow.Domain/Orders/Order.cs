namespace OrderFlow.Domain.Orders;

public class Order
{
    private readonly List<OrderItem> MutableItems = [];

    public Guid Id { get; private set; }

    public Guid CustomerId { get; private set; }

    public IReadOnlyCollection<OrderItem> Items =>
        MutableItems.AsReadOnly();

    public decimal Total =>
        MutableItems.Sum(item => item.Subtotal);

    public OrderStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    protected Order()
    {
    }

    public Order(Guid customerId)
    {
        if (customerId == Guid.Empty)
        {
            throw new ArgumentException(
                "Customer identifier is required.",
                nameof(customerId));
        }

        var now = DateTime.UtcNow;

        Id = Guid.NewGuid();
        CustomerId = customerId;
        Status = OrderStatus.Created;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Status != OrderStatus.Created)
        {
            throw new InvalidOperationException(
                "Items can only be added while the order is being created.");
        }

        if (MutableItems.Any(
            existingItem =>
                existingItem.ProductId == item.ProductId))
        {
            throw new InvalidOperationException(
                "The product is already included in the order.");
        }

        MutableItems.Add(item);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (!OrderStatusTransition.IsAllowed(Status, newStatus))
        {
            throw new InvalidOperationException(
                $"Transition from {Status} to {newStatus} is not allowed.");
        }

        if (newStatus == OrderStatus.Confirmed &&
            MutableItems.Count == 0)
        {
            throw new InvalidOperationException(
                "An empty order cannot be confirmed.");
        }

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
    }
}