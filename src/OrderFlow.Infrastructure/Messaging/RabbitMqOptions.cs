namespace OrderFlow.Infrastructure.Messaging;

public sealed class RabbitMqOptions
{
    public string HostName { get; init; } = string.Empty;

    public int Port { get; init; }

    public string UserName { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    public string VirtualHost { get; init; } = string.Empty;

    public string ExchangeName { get; init; } = string.Empty;

    public string OrderCreatedRoutingKey { get; init; } = string.Empty;

    public string OrderCreatedQueue { get; init; } = string.Empty;

    public string DeadLetterExchangeName { get; init; } = string.Empty;

    public string DeadLetterQueueName { get; init; } = string.Empty;

    public ushort PrefetchCount { get; init; }

    public string ClientProvidedName { get; init; } = string.Empty;

    public int NetworkRecoverySeconds { get; init; }

    public int RequestedHeartbeatSeconds { get; init; }
}