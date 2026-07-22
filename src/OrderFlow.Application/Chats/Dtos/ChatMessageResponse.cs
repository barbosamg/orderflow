namespace OrderFlow.Application.Chats.Dtos;

public sealed record ChatMessageResponse(
    Guid Id,
    Guid OrderId,
    string Sender,
    string Text,
    DateTimeOffset SentAt);