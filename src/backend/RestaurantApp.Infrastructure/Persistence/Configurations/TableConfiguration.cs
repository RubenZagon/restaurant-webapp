using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Table entity
/// </summary>
public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("tables");

        // Primary key configuration with value object conversion
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasConversion(
                id => id.Value,
                value => new TableId(value))
            .HasColumnName("id")
            .IsRequired();

        // Owned entity configuration for TableSession
        builder.OwnsOne(t => t.ActiveSession, session =>
        {
            session.Property(s => s.Id)
                .HasConversion(
                    id => id.Value,
                    value => SessionId.From(value))
                .HasColumnName("active_session_id")
                .IsRequired();

            session.Property(s => s.StartedAt)
                .HasColumnName("session_started_at")
                .IsRequired();

            session.Property(s => s.EndedAt)
                .HasColumnName("session_ended_at")
                .IsRequired(false);

            // Ignore computed properties
            session.Ignore(s => s.IsActive);
        });

        // Ignore computed property
        builder.Ignore(t => t.IsOccupied);
    }
}
