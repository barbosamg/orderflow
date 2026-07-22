namespace OrderFlow.Application.Chats.Dtos;

public sealed record SendChatMessageRequest(
    string Sender,
    string Text);