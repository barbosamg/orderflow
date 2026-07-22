using OrderFlow.Domain.Orders;

namespace OrderFlow.Application.Orders.Dtos;

public sealed record ChangeOrderStatusRequest(
    OrderStatus Status);