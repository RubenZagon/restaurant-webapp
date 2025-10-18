using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using RestaurantApp.Application.Services;
using RestaurantApp.Domain.Events;

namespace RestaurantApp.Infrastructure.Services;

/// <summary>
/// Implementation of order notification service using SignalR
/// </summary>
public class OrderNotificationService : IOrderNotificationService
{
    private readonly IHubContext<OrderNotificationHub> _hubContext;
    private readonly ILogger<OrderNotificationService> _logger;

    public OrderNotificationService(
        IHubContext<OrderNotificationHub> hubContext,
        ILogger<OrderNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyOrderConfirmed(OrderConfirmedEvent orderEvent)
    {
        _logger.LogInformation(
            "Notifying order confirmed: OrderId={OrderId}, Table={TableNumber}",
            orderEvent.OrderId.Value,
            orderEvent.TableNumber);

        var notification = new
        {
            type = "OrderConfirmed",
            orderId = orderEvent.OrderId.Value.ToString(),
            tableNumber = orderEvent.TableNumber,
            occurredAt = orderEvent.OccurredOn
        };

        // Notify the specific table
        var tableGroup = $"table_{orderEvent.TableNumber}";
        await _hubContext.Clients.Group(tableGroup)
            .SendAsync("OrderConfirmed", notification);

        // Notify kitchen staff
        await _hubContext.Clients.Group("kitchen")
            .SendAsync("NewOrder", notification);

        _logger.LogInformation("Order confirmed notification sent successfully");
    }

    public async Task NotifyOrderStatusChanged(OrderStatusChangedEvent statusEvent)
    {
        _logger.LogInformation(
            "Notifying order status changed: OrderId={OrderId}, Table={TableNumber}, From={OldStatus}, To={NewStatus}",
            statusEvent.OrderId.Value,
            statusEvent.TableNumber,
            statusEvent.OldStatus,
            statusEvent.NewStatus);

        var notification = new
        {
            type = "OrderStatusChanged",
            orderId = statusEvent.OrderId.Value.ToString(),
            tableNumber = statusEvent.TableNumber,
            oldStatus = statusEvent.OldStatus.ToString(),
            newStatus = statusEvent.NewStatus.ToString(),
            occurredAt = statusEvent.OccurredOn
        };

        // Notify the specific table
        var tableGroup = $"table_{statusEvent.TableNumber}";
        await _hubContext.Clients.Group(tableGroup)
            .SendAsync("OrderStatusChanged", notification);

        // Notify kitchen staff
        await _hubContext.Clients.Group("kitchen")
            .SendAsync("OrderStatusChanged", notification);

        _logger.LogInformation("Order status changed notification sent successfully");
    }
}

// Hub class needs to be accessible - add it to Infrastructure
public class OrderNotificationHub : Hub
{
    private readonly ILogger<OrderNotificationHub> _logger;

    public OrderNotificationHub(ILogger<OrderNotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToTable(int tableNumber)
    {
        var groupName = $"table_{tableNumber}";
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} subscribed to table {TableNumber}",
            Context.ConnectionId, tableNumber);
    }

    public async Task UnsubscribeFromTable(int tableNumber)
    {
        var groupName = $"table_{tableNumber}";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        _logger.LogInformation("Client {ConnectionId} unsubscribed from table {TableNumber}",
            Context.ConnectionId, tableNumber);
    }

    public async Task SubscribeToKitchen()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "kitchen");
        _logger.LogInformation("Client {ConnectionId} subscribed to kitchen notifications",
            Context.ConnectionId);
    }

    public async Task UnsubscribeFromKitchen()
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "kitchen");
        _logger.LogInformation("Client {ConnectionId} unsubscribed from kitchen notifications",
            Context.ConnectionId);
    }
}
