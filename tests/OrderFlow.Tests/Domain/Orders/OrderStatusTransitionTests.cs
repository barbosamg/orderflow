using OrderFlow.Domain.Orders;

namespace OrderFlow.Tests.Domain.Orders;

public class OrderStatusTransitionTests
{
    [Theory]
    [InlineData(OrderStatus.Created, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Created, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Preparing)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Cancelled)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Delivered)]
    public void IsAllowed_ShouldReturnTrue_ForValidTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        var result = OrderStatusTransition.IsAllowed(
            currentStatus,
            newStatus);

        Assert.True(result);
    }

        [Theory]
    [InlineData(OrderStatus.Created, OrderStatus.Created)]
    [InlineData(OrderStatus.Created, OrderStatus.Preparing)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Created)]
    [InlineData(OrderStatus.Confirmed, OrderStatus.Shipped)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Preparing, OrderStatus.Delivered)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Preparing)]
    [InlineData(OrderStatus.Shipped, OrderStatus.Cancelled)]
    public void IsAllowed_ShouldReturnFalse_ForInvalidTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        var result = OrderStatusTransition.IsAllowed(
            currentStatus,
            newStatus);

        Assert.False(result);
    }

    [Theory]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.Cancelled)]
    public void IsAllowed_ShouldReturnFalse_ForEveryTransitionFromFinalStatus(
        OrderStatus finalStatus)
    {
        foreach (var newStatus in Enum.GetValues<OrderStatus>())
        {
            var result = OrderStatusTransition.IsAllowed(
                finalStatus,
                newStatus);

            Assert.False(result);
        }
    }
}