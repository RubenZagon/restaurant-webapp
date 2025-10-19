using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

public class Table
{
    public TableId Id { get; private set; }
    public TableSession? ActiveSession { get; private set; }
    public bool IsOccupied => ActiveSession?.IsActive ?? false;

    // Parameterless constructor for EF Core
    private Table()
    {
        Id = null!; // Will be set by EF Core
    }

    public Table(TableId id)
    {
        Id = id;
        ActiveSession = null;
    }

    public void StartSession()
    {
        // If there's no active session, create one
        // If there's already an active session, keep using it (allows multiple people at same table)
        if (!IsOccupied)
        {
            ActiveSession = TableSession.Create();
        }
    }

    public void EndSession()
    {
        if (!IsOccupied)
        {
            throw new DomainException(
                $"Table {Id.Value} does not have an active session to end.");
        }

        ActiveSession!.End();
        ActiveSession = null;
    }
}
