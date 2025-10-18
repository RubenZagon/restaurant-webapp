using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Events;

/// <summary>
/// Event raised when an order is confirmed
/// </summary>
public record OrderConfirmedEvent : IDomainEvent
{
    public OrderId OrderId { get; }
    public int TableNumber { get; }
    public DateTime OccurredOn { get; }

    public OrderConfirmedEvent(OrderId orderId, int tableNumber)
    {
        OrderId = orderId;
        TableNumber = tableNumber;
        OccurredOn = DateTime.UtcNow;
    }
}
