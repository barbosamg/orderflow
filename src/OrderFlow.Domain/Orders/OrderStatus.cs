namespace OrderFlow.Domain.Orders;

public enum OrderStatus
{
    Created,
    Confirmed,
    Preparing,
    Shipped,
    Delivered,
    Cancelled
}