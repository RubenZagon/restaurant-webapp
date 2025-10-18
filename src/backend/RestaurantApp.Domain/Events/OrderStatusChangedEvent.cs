using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Events;

/// <summary>
/// Event raised when an order status changes
/// </summary>
public record OrderStatusChangedEvent : IDomainEvent
{
    public OrderId OrderId { get; }
    public int TableNumber { get; }
    public OrderStatus OldStatus { get; }
    public OrderStatus NewStatus { get; }
    public DateTime OccurredOn { get; }

    public OrderStatusChangedEvent(
        OrderId orderId,
        int tableNumber,
        OrderStatus oldStatus,
        OrderStatus newStatus)
    {
        OrderId = orderId;
        TableNumber = tableNumber;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        OccurredOn = DateTime.UtcNow;
    }
}
