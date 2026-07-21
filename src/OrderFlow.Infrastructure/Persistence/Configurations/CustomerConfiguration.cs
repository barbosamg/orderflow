using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Domain.Customers;

namespace OrderFlow.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration :
    IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("customers");

        builder.HasKey(customer => customer.Id)
            .HasName("pk_customers");

        builder.Property(customer => customer.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(customer => customer.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(customer => customer.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(customer => customer.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(customer => customer.Email)
            .IsUnique()
            .HasDatabaseName("ix_customers_email");
    }
}