using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Chats;
using OrderFlow.Domain.Orders;

namespace OrderFlow.Infrastructure.Persistence.Configurations;

public class ChatMessageConfiguration :
    IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("chat_messages");

        builder.HasKey(message => message.Id)
            .HasName("pk_chat_messages");

        builder.Property(message => message.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(message => message.OrderId)
            .HasColumnName("order_id")
            .IsRequired();

        builder.Property(message => message.Sender)
            .HasColumnName("sender")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(message => message.Text)
            .HasColumnName("text")
            .HasMaxLength(1_000)
            .IsRequired();

        builder.Property(message => message.SentAt)
            .HasColumnName("sent_at")
            .IsRequired();

        builder.HasOne<Order>()
            .WithMany()
            .HasForeignKey(message => message.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(message => new
            {
                message.OrderId,
                message.SentAt
            })
            .HasDatabaseName("ix_chat_messages_order_id_sent_at");
    }
}