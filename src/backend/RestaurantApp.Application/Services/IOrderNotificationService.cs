using RestaurantApp.Domain.Events;

namespace RestaurantApp.Application.Services;

/// <summary>
/// Service for sending order notifications via SignalR
/// </summary>
public interface IOrderNotificationService
{
    Task NotifyOrderConfirmed(OrderConfirmedEvent orderEvent);
    Task NotifyOrderStatusChanged(OrderStatusChangedEvent statusEvent);
}
