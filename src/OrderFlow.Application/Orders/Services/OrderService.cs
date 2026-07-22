using OrderFlow.Application.Common.Contracts;
using OrderFlow.Application.Customers.Repositories;
using OrderFlow.Application.Orders.Contracts;
using OrderFlow.Application.Orders.Dtos;
using OrderFlow.Application.Orders.Events;
using OrderFlow.Application.Orders.Exceptions;
using OrderFlow.Application.Orders.Mappings;
using OrderFlow.Application.Orders.Repositories;
using OrderFlow.Application.Products.Repositories;
using OrderFlow.Domain.Orders;
using OrderFlow.Domain.Products;

namespace OrderFlow.Application.Orders.Services;

public sealed class OrderService : IOrderService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderCreatedPublisher _orderCreatedPublisher;

    public OrderService(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IOrderCreatedPublisher orderCreatedPublisher)
    {
        _customerRepository = customerRepository ??
            throw new ArgumentNullException(nameof(customerRepository));

        _productRepository = productRepository ??
            throw new ArgumentNullException(nameof(productRepository));

        _orderRepository = orderRepository ??
            throw new ArgumentNullException(nameof(orderRepository));

        _unitOfWork = unitOfWork ??
            throw new ArgumentNullException(nameof(unitOfWork));

        _orderCreatedPublisher = orderCreatedPublisher ??
            throw new ArgumentNullException(nameof(orderCreatedPublisher));
    }

    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateCreateRequest(request);

        var order = await _unitOfWork.ExecuteInTransactionAsync(
            transactionCancellationToken =>
                CreateWithinTransactionAsync(
                    request,
                    transactionCancellationToken),
            cancellationToken);

        var orderCreatedEvent = new OrderCreatedEvent(
            Guid.NewGuid(),
            order.Id,
            order.CustomerId,
            order.Total,
            order.CreatedAt);

        await _orderCreatedPublisher.PublishAsync(
            orderCreatedEvent,
            cancellationToken);

        return order.ToResponse();
    }

    public async Task<OrderResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            id,
            cancellationToken);

        return order?.ToResponse();
    }

    public async Task<OrderResponse?> ChangeStatusAsync(
        Guid id,
        ChangeOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var order = await _orderRepository.GetByIdForUpdateAsync(
            id,
            cancellationToken);

        if (order is null)
        {
            return null;
        }

        try
        {
            order.ChangeStatus(request.Status);
        }
        catch (InvalidOperationException exception)
        {
            throw new OrderConflictException(exception.Message);
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return order.ToResponse();
    }

    private async Task<Order> CreateWithinTransactionAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(
            request.CustomerId,
            cancellationToken);

        if (customer is null)
        {
            throw new OrderResourceNotFoundException(
                "The requested customer was not found.");
        }

        var productIds = request.Items
            .Select(item => item.ProductId)
            .ToArray();

        var products = await _productRepository.GetByIdsAsync(
            productIds,
            cancellationToken);

        if (products.Count != productIds.Length)
        {
            throw new OrderResourceNotFoundException(
                "One or more requested products were not found.");
        }

        var productsById = products.ToDictionary(
            product => product.Id);

        ValidateProducts(
            request.Items,
            productsById);

        var order = new Order(customer.Id);

        foreach (var requestedItem in request.Items)
        {
            var product = productsById[requestedItem.ProductId];

            var orderItem = new OrderItem(
                product.Id,
                product.Name,
                product.Price,
                requestedItem.Quantity);

            product.DecreaseStock(requestedItem.Quantity);
            order.AddItem(orderItem);
        }

        await _orderRepository.AddAsync(
            order,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return order;
    }

    private static void ValidateCreateRequest(
        CreateOrderRequest request)
    {
        if (request.CustomerId == Guid.Empty)
        {
            throw new OrderValidationException(
                "Customer identifier is required.");
        }

        if (request.Items is null ||
            request.Items.Count == 0)
        {
            throw new OrderValidationException(
                "The order must contain at least one item.");
        }

        foreach (var item in request.Items)
        {
            if (item.ProductId == Guid.Empty)
            {
                throw new OrderValidationException(
                    "Product identifier is required.");
            }

            if (item.Quantity <= 0)
            {
                throw new OrderValidationException(
                    "Item quantity must be greater than zero.");
            }
        }

        var hasRepeatedProduct = request.Items
            .GroupBy(item => item.ProductId)
            .Any(group => group.Count() > 1);

        if (hasRepeatedProduct)
        {
            throw new OrderValidationException(
                "The same product cannot be included more than once.");
        }
    }

    private static void ValidateProducts(
        IReadOnlyCollection<CreateOrderItemRequest> requestedItems,
        IReadOnlyDictionary<Guid, Product> productsById)
    {
        foreach (var requestedItem in requestedItems)
        {
            var product = productsById[requestedItem.ProductId];

            if (!product.IsActive)
            {
                throw new OrderConflictException(
                    "One or more requested products are inactive.");
            }

            if (product.Stock < requestedItem.Quantity)
            {
                throw new OrderConflictException(
                    "One or more requested products have insufficient stock.");
            }
        }
    }
}