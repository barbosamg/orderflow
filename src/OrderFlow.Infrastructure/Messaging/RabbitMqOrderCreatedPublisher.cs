using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderFlow.Application.Orders.Events;
using RabbitMQ.Client;

namespace OrderFlow.Infrastructure.Messaging;

public sealed class RabbitMqOrderCreatedPublisher :
    IOrderCreatedPublisher,
    IAsyncDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqOrderCreatedPublisher> _logger;
    private readonly SemaphoreSlim _channelSemaphore = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;
    private bool _disposed;

    public RabbitMqOrderCreatedPublisher(
        RabbitMqOptions options,
        ILogger<RabbitMqOrderCreatedPublisher> logger)
    {
        _options = options ??
            throw new ArgumentNullException(nameof(options));

        _logger = logger ??
            throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishAsync(
        OrderCreatedEvent orderCreatedEvent,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(orderCreatedEvent);

        ThrowIfDisposed();

        var body = JsonSerializer.SerializeToUtf8Bytes(
            orderCreatedEvent);

        await _channelSemaphore.WaitAsync(cancellationToken);

        try
        {
            ThrowIfDisposed();

            var channel = await GetOrCreateChannelAsync(
                cancellationToken);

            var properties = new BasicProperties
            {
                AppId = "orderflow-api",
                ContentType = "application/json",
                ContentEncoding = "utf-8",
                MessageId = orderCreatedEvent.EventId.ToString(),
                Type = "order.created",
                Persistent = true,
                Timestamp = new AmqpTimestamp(
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: _options.ExchangeName,
                routingKey: _options.OrderCreatedRoutingKey,
                mandatory: true,
                basicProperties: properties,
                body: body,
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Order created event {EventId} published for order {OrderId}.",
                orderCreatedEvent.EventId,
                orderCreatedEvent.OrderId);
        }
        finally
        {
            _channelSemaphore.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        await _channelSemaphore.WaitAsync(CancellationToken.None);

        try
        {
            if (_channel is not null)
            {
                if (_channel.IsOpen)
                {
                    await _channel.CloseAsync(
                        CancellationToken.None);
                }

                await _channel.DisposeAsync();
                _channel = null;
            }

            if (_connection is not null)
            {
                if (_connection.IsOpen)
                {
                    await _connection.CloseAsync(
                        CancellationToken.None);
                }

                await _connection.DisposeAsync();
                _connection = null;
            }
        }
        finally
        {
            _channelSemaphore.Release();
            _channelSemaphore.Dispose();
        }
    }

    private async Task<IChannel> GetOrCreateChannelAsync(
        CancellationToken cancellationToken)
    {
        if (_connection is null ||
            !_connection.IsOpen)
        {
            if (_connection is not null)
            {
                await _connection.DisposeAsync();
            }

            var connectionFactory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password,
                VirtualHost = _options.VirtualHost,
                ClientProvidedName = _options.ClientProvidedName,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(
                    _options.NetworkRecoverySeconds),
                RequestedHeartbeat = TimeSpan.FromSeconds(
                    _options.RequestedHeartbeatSeconds)
            };

            _connection =
                await connectionFactory.CreateConnectionAsync(
                    cancellationToken);
        }

        if (_channel is null ||
            !_channel.IsOpen)
        {
            if (_channel is not null)
            {
                await _channel.DisposeAsync();
            }

            var channelOptions = new CreateChannelOptions(
                publisherConfirmationsEnabled: true,
                publisherConfirmationTrackingEnabled: true);

            _channel = await _connection.CreateChannelAsync(
                channelOptions,
                cancellationToken);

            await _channel.ExchangeDeclareAsync(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken);
        }

        return _channel;
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            _disposed,
            this);
    }
}