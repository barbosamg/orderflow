namespace OrderFlow.Application.Orders.Exceptions;

public sealed class OrderConflictException : Exception
{
    public OrderConflictException(string message)
        : base(message)
    {
    }
}