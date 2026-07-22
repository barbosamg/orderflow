namespace OrderFlow.Application.Orders.Exceptions;

public sealed class OrderResourceNotFoundException : Exception
{
    public OrderResourceNotFoundException(string message)
        : base(message)
    {
    }
}