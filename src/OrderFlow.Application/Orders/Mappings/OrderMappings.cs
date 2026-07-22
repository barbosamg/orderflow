using OrderFlow.Application.Orders.Dtos;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.Mappings;

internal static class OrderMappings
{
    public static OrderResponse ToResponse(this Order order)
    {
        var items = order.Items
            .Select(
                item => new OrderItemResponse(
                    item.Id,
                    item.ProductId,
                    item.ProductName,
                    item.UnitPrice,
                    item.Quantity,
                    item.Subtotal))
            .ToArray();

        return new OrderResponse(
            order.Id,
            order.CustomerId,
            items,
            order.Total,
            order.Status,
            order.CreatedAt,
            order.UpdatedAt);
    }
}