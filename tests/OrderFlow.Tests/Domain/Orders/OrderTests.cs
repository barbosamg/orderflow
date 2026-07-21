using OrderFlow.Domain.Orders;

namespace OrderFlow.Tests.Domain.Orders;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldCreateOrder_WithInitialState()
    {
        var customerId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        var order = new Order(customerId);

        var afterCreation = DateTime.UtcNow;

        Assert.NotEqual(Guid.Empty, order.Id);
        Assert.Equal(customerId, order.CustomerId);
        Assert.Empty(order.Items);
        Assert.Equal(0m, order.Total);
        Assert.Equal(OrderStatus.Created, order.Status);
        Assert.InRange(
            order.CreatedAt,
            beforeCreation,
            afterCreation);
        Assert.Equal(order.CreatedAt, order.UpdatedAt);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenCustomerIdIsEmpty()
    {
        var exception = Assert.Throws<ArgumentException>(
            () => new Order(Guid.Empty));

        Assert.Equal("customerId", exception.ParamName);
    }

    [Fact]
    public void AddItem_ShouldIncludeItemAndCalculateTotal()
    {
        var order = new Order(Guid.NewGuid());
        var item = new OrderItem(
            Guid.NewGuid(),
            "Coffee",
            12.50m,
            2);

        order.AddItem(item);

        Assert.Single(order.Items);
        Assert.Contains(item, order.Items);
        Assert.Equal(25.00m, order.Total);
    }

    [Fact]
    public void AddItem_ShouldCalculateTotal_ForMultipleItems()
    {
        var order = new Order(Guid.NewGuid());

        order.AddItem(
            new OrderItem(
                Guid.NewGuid(),
                "Coffee",
                12.50m,
                2));

        order.AddItem(
            new OrderItem(
                Guid.NewGuid(),
                "Cake",
                8.75m,
                3));

        Assert.Equal(2, order.Items.Count);
        Assert.Equal(51.25m, order.Total);
    }

    [Fact]
    public void AddItem_ShouldThrow_WhenItemIsNull()
    {
        var order = new Order(Guid.NewGuid());

        Assert.Throws<ArgumentNullException>(
            () => order.AddItem(null!));
    }

    [Fact]
    public void AddItem_ShouldThrow_WhenProductAlreadyExists()
    {
        var order = new Order(Guid.NewGuid());
        var productId = Guid.NewGuid();

        order.AddItem(
            new OrderItem(
                productId,
                "Coffee",
                10m,
                1));

        var duplicateItem = new OrderItem(
            productId,
            "Coffee",
            10m,
            2);

        Assert.Throws<InvalidOperationException>(
            () => order.AddItem(duplicateItem));

        Assert.Single(order.Items);
        Assert.Equal(10m, order.Total);
    }

    [Fact]
    public void ChangeStatus_ShouldConfirmOrder_WhenItHasItems()
    {
        var order = new Order(Guid.NewGuid());

        order.AddItem(
            new OrderItem(
                Guid.NewGuid(),
                "Coffee",
                10m,
                1));

        var beforeChange = DateTime.UtcNow;

        order.ChangeStatus(OrderStatus.Confirmed);

        var afterChange = DateTime.UtcNow;

        Assert.Equal(OrderStatus.Confirmed, order.Status);
        Assert.InRange(
            order.UpdatedAt,
            beforeChange,
            afterChange);
    }

    [Fact]
    public void ChangeStatus_ShouldThrow_WhenConfirmingEmptyOrder()
    {
        var order = new Order(Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(
            () => order.ChangeStatus(OrderStatus.Confirmed));

        Assert.Equal(OrderStatus.Created, order.Status);
    }

    [Fact]
    public void ChangeStatus_ShouldThrow_WhenTransitionIsInvalid()
    {
        var order = new Order(Guid.NewGuid());

        order.AddItem(
            new OrderItem(
                Guid.NewGuid(),
                "Coffee",
                10m,
                1));

        Assert.Throws<InvalidOperationException>(
            () => order.ChangeStatus(OrderStatus.Preparing));

        Assert.Equal(OrderStatus.Created, order.Status);
    }

    [Fact]
    public void AddItem_ShouldThrow_WhenOrderIsNotCreated()
    {
        var order = new Order(Guid.NewGuid());

        order.AddItem(
            new OrderItem(
                Guid.NewGuid(),
                "Coffee",
                10m,
                1));

        order.ChangeStatus(OrderStatus.Confirmed);

        var anotherItem = new OrderItem(
            Guid.NewGuid(),
            "Cake",
            8m,
            1);

        Assert.Throws<InvalidOperationException>(
            () => order.AddItem(anotherItem));

        Assert.Single(order.Items);
    }

    [Fact]
    public void ChangeStatus_ShouldThrow_AfterOrderIsCancelled()
    {
        var order = new Order(Guid.NewGuid());

        order.ChangeStatus(OrderStatus.Cancelled);

        Assert.Throws<InvalidOperationException>(
            () => order.ChangeStatus(OrderStatus.Confirmed));

        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

}