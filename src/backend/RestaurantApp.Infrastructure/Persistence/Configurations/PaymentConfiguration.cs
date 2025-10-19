using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Payment entity
/// </summary>
public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");

        // Primary key configuration with value object conversion
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => new PaymentId(value))
            .HasColumnName("id")
            .IsRequired();

        // OrderId value object conversion
        builder.Property(p => p.OrderId)
            .HasConversion(
                id => id.Value,
                value => OrderId.From(value))
            .HasColumnName("order_id")
            .IsRequired();

        // Amount as owned value object
        builder.OwnsOne(p => p.Amount, amount =>
        {
            amount.Property(a => a.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            amount.Property(a => a.Currency)
                .HasColumnName("currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // PaymentStatus enum stored as string
        builder.Property(p => p.Status)
            .HasConversion<string>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        // String properties
        builder.Property(p => p.TransactionId)
            .HasColumnName("transaction_id")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(p => p.FailureReason)
            .HasColumnName("failure_reason")
            .HasMaxLength(500)
            .IsRequired(false);

        // DateTime properties
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.ProcessedAt)
            .HasColumnName("processed_at")
            .IsRequired(false);

        // Indexes for performance
        builder.HasIndex(p => p.OrderId)
            .HasDatabaseName("ix_payments_order_id");

        builder.HasIndex(p => p.Status)
            .HasDatabaseName("ix_payments_status");

        builder.HasIndex(p => p.TransactionId)
            .HasDatabaseName("ix_payments_transaction_id");

        builder.HasIndex(p => p.CreatedAt)
            .HasDatabaseName("ix_payments_created_at");
    }
}
