using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

public class Table
{
    public TableId Id { get; private set; }
    public TableSession? ActiveSession { get; private set; }
    public bool IsOccupied => ActiveSession?.IsActive ?? false;

    public Table(TableId id)
    {
        Id = id;
        ActiveSession = null;
    }

    public void StartSession()
    {
        if (IsOccupied)
        {
            throw new DomainException(
                $"Table {Id.Value} already has an active session.");
        }

        ActiveSession = TableSession.Create();
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
