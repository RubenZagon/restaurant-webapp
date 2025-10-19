using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Category entity
/// </summary>
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        // Primary key configuration with value object conversion
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasConversion(
                id => id.Value,
                value => CategoryId.From(value))
            .HasColumnName("id")
            .IsRequired();

        // String properties
        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired(false);

        // Boolean property
        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes for performance
        builder.HasIndex(c => c.Name)
            .HasDatabaseName("ix_categories_name");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("ix_categories_is_active");
    }
}
