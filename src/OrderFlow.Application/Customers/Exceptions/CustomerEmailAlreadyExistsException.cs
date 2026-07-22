namespace OrderFlow.Application.Customers.Exceptions;

public sealed class CustomerEmailAlreadyExistsException : Exception
{
    public CustomerEmailAlreadyExistsException()
        : base("A customer with the provided email already exists.")
    {
    }
}