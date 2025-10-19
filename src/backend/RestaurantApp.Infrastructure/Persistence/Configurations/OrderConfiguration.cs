using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Order entity
/// </summary>
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("orders");

        // Primary key configuration with value object conversion
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value,
                value => OrderId.From(value))
            .HasColumnName("id")
            .IsRequired();

        // TableId value object conversion
        builder.Property(o => o.TableId)
            .HasConversion(
                id => id.Value,
                value => new TableId(value))
            .HasColumnName("table_id")
            .IsRequired();

        // SessionId value object conversion
        builder.Property(o => o.SessionId)
            .HasConversion(
                id => id.Value,
                value => SessionId.From(value))
            .HasColumnName("session_id")
            .IsRequired();

        // OrderStatus enum
        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();

        // Total price as owned value object
        builder.OwnsOne(o => o.Total, total =>
        {
            total.Property(t => t.Amount)
                .HasColumnName("total_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            total.Property(t => t.Currency)
                .HasColumnName("total_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // DateTime properties
        builder.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(o => o.ConfirmedAt)
            .HasColumnName("confirmed_at")
            .IsRequired(false);

        // Configure owned collection of OrderLines
        builder.OwnsMany(o => o.Lines, line =>
        {
            line.ToTable("order_lines");

            // Primary key for order line
            line.Property(l => l.Id)
                .HasConversion(
                    id => id.Value,
                    value => OrderLineId.From(value))
                .HasColumnName("id")
                .IsRequired();

            line.HasKey(nameof(OrderLine.Id));

            // Foreign key to Order
            line.WithOwner()
                .HasForeignKey("order_id");

            // ProductId value object conversion
            line.Property(l => l.ProductId)
                .HasConversion(
                    id => id.Value,
                    value => ProductId.From(value))
                .HasColumnName("product_id")
                .IsRequired();

            // Product name
            line.Property(l => l.ProductName)
                .HasColumnName("product_name")
                .HasMaxLength(200)
                .IsRequired();

            // UnitPrice as owned value object
            line.OwnsOne(l => l.UnitPrice, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("unit_price_amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("unit_price_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Quantity value object conversion
            line.Property(l => l.Quantity)
                .HasConversion(
                    quantity => quantity.Value,
                    value => new Quantity(value))
                .HasColumnName("quantity")
                .IsRequired();

            // Subtotal as owned value object
            line.OwnsOne(l => l.Subtotal, subtotal =>
            {
                subtotal.Property(s => s.Amount)
                    .HasColumnName("subtotal_amount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                subtotal.Property(s => s.Currency)
                    .HasColumnName("subtotal_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            // Index for ProductId
            line.HasIndex(l => l.ProductId)
                .HasDatabaseName("ix_order_lines_product_id");
        });

        // Indexes for performance
        builder.HasIndex(o => o.TableId)
            .HasDatabaseName("ix_orders_table_id");

        builder.HasIndex(o => o.SessionId)
            .HasDatabaseName("ix_orders_session_id");

        builder.HasIndex(o => o.Status)
            .HasDatabaseName("ix_orders_status");

        builder.HasIndex(o => o.CreatedAt)
            .HasDatabaseName("ix_orders_created_at");
    }
}
