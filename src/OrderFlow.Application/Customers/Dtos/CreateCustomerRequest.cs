namespace OrderFlow.Application.Customers.Dtos;

public sealed record CreateCustomerRequest(
    string Name,
    string Email);