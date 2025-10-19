using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

public class TableSession
{
    public SessionId Id { get; private set; }
    public DateTime StartedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public bool IsActive => EndedAt == null;

    // Parameterless constructor for EF Core
    private TableSession()
    {
        Id = null!; // Will be set by EF Core
    }

    private TableSession(SessionId id, DateTime startedAt)
    {
        Id = id;
        StartedAt = startedAt;
        EndedAt = null;
    }

    public static TableSession Create()
    {
        return new TableSession(SessionId.Create(), DateTime.UtcNow);
    }

    public void End()
    {
        EndedAt = DateTime.UtcNow;
    }
}
