using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Events;

namespace OrderFlow.Infrastructure.Persistence.Configurations;

public class ProcessedEventConfiguration :
    IEntityTypeConfiguration<ProcessedEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedEvent> builder)
    {
        builder.ToTable("processed_events");

        builder.HasKey(processedEvent => processedEvent.EventId)
            .HasName("pk_processed_events");

        builder.Property(processedEvent => processedEvent.EventId)
            .HasColumnName("event_id")
            .ValueGeneratedNever();

        builder.Property(processedEvent => processedEvent.EventType)
            .HasColumnName("event_type")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(processedEvent => processedEvent.Payload)
            .HasColumnName("payload")
            .IsRequired();

        builder.Property(processedEvent => processedEvent.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired();
    }
}