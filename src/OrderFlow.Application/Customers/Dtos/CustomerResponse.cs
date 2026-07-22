namespace OrderFlow.Application.Customers.Dtos;

public sealed record CustomerResponse(
    Guid Id,
    string Name,
    string Email,
    DateTimeOffset CreatedAt);