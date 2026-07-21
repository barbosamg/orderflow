namespace OrderFlow.Domain.Orders;

public static class OrderStatusTransition
{
    public static bool IsAllowed(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Created =>
                newStatus is OrderStatus.Confirmed or OrderStatus.Cancelled,

            OrderStatus.Confirmed =>
                newStatus is OrderStatus.Preparing or OrderStatus.Cancelled,

            OrderStatus.Preparing =>
                newStatus is OrderStatus.Shipped or OrderStatus.Cancelled,

            OrderStatus.Shipped =>
                newStatus == OrderStatus.Delivered,

            OrderStatus.Delivered or OrderStatus.Cancelled => false,

            _ => false
        };
    }
}