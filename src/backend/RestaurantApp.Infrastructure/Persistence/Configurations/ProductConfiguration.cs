using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Product entity
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        // Primary key configuration with value object conversion
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                value => ProductId.From(value))
            .HasColumnName("id")
            .IsRequired();

        // String properties
        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasMaxLength(1000)
            .IsRequired(false);

        // Price value object configuration - stored as separate columns
        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(pr => pr.Amount)
                .HasColumnName("price_amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            price.Property(pr => pr.Currency)
                .HasColumnName("price_currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // CategoryId value object conversion
        builder.Property(p => p.CategoryId)
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value))
            .HasColumnName("category_id")
            .IsRequired();

        // Allergens value object - stored as JSON array
        builder.Property(p => p.Allergens)
            .HasConversion(
                allergens => string.Join(",", allergens.Values),
                value => new Allergens(
                    string.IsNullOrWhiteSpace(value)
                        ? Array.Empty<string>()
                        : value.Split(',', StringSplitOptions.RemoveEmptyEntries)))
            .HasColumnName("allergens")
            .HasMaxLength(500)
            .IsRequired(false);

        // Boolean property
        builder.Property(p => p.IsAvailable)
            .HasColumnName("is_available")
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes for performance
        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("ix_products_category_id");

        builder.HasIndex(p => p.IsAvailable)
            .HasDatabaseName("ix_products_is_available");

        builder.HasIndex(p => p.Name)
            .HasDatabaseName("ix_products_name");
    }
}
