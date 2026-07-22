using OrderFlow.Application.Customers.Dtos;
using OrderFlow.Domain.Customers;

namespace OrderFlow.Application.Customers.Mappings;

internal static class CustomerMappings
{
    public static CustomerResponse ToResponse(this Customer customer)
    {
        return new CustomerResponse(
            customer.Id,
            customer.Name,
            customer.Email,
            customer.CreatedAt);
    }
}