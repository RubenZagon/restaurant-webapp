using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Application.Services;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Events;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

/// <summary>
/// Use case for updating order status (kitchen workflow)
/// Handles transitions: Confirmed → Preparing → Ready → Delivered
/// </summary>
public class UpdateOrderStatusUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderNotificationService _notificationService;

    public UpdateOrderStatusUseCase(
        IOrderRepository orderRepository,
        IOrderNotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _notificationService = notificationService;
    }

    public async Task<Result<OrderDto>> Execute(Guid orderId, string newStatus)
    {
        // 1. Validate order exists
        var order = await _orderRepository.GetById(OrderId.From(orderId));
        if (order == null)
        {
            return Result<OrderDto>.Failure("Order not found");
        }

        // 2. Validate status is valid
        if (!IsValidStatus(newStatus))
        {
            return Result<OrderDto>.Failure($"Invalid status: {newStatus}. Valid statuses are: Preparing, Ready, Delivered");
        }

        // 3. Apply status transition
        try
        {
            ApplyStatusTransition(order, newStatus);
        }
        catch (DomainException ex)
        {
            return Result<OrderDto>.Failure(ex.Message);
        }

        // 4. Save order
        await _orderRepository.Save(order);

        // 5. Dispatch domain events
        await DispatchDomainEvents(order);

        // 6. Return updated order
        var dto = MapToDto(order);
        return Result<OrderDto>.Success(dto);
    }

    private static bool IsValidStatus(string status)
    {
        return status is "Preparing" or "Ready" or "Delivered";
    }

    private static void ApplyStatusTransition(Order order, string newStatus)
    {
        switch (newStatus)
        {
            case "Preparing":
                order.MarkAsPreparing();
                break;
            case "Ready":
                order.MarkAsReady();
                break;
            case "Delivered":
                order.MarkAsDelivered();
                break;
            default:
                throw new DomainException($"Invalid status: {newStatus}");
        }
    }

    private async Task DispatchDomainEvents(Order order)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            if (domainEvent is OrderStatusChangedEvent statusChangedEvent)
            {
                await _notificationService.NotifyOrderStatusChanged(statusChangedEvent);
            }
        }
        order.ClearDomainEvents();
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto(
            Id: order.Id.Value,
            TableNumber: order.TableId.Value,
            SessionId: order.SessionId.Value,
            Lines: order.Lines.Select(l => new OrderLineDto(
                Id: l.Id.Value,
                ProductId: l.ProductId.Value,
                ProductName: l.ProductName,
                UnitPrice: l.UnitPrice.Amount,
                Quantity: l.Quantity.Value,
                Subtotal: l.Subtotal.Amount
            )).ToList(),
            Status: order.Status.ToString(),
            Total: order.Total.Amount,
            Currency: order.Total.Currency,
            CreatedAt: order.CreatedAt,
            ConfirmedAt: order.ConfirmedAt
        );
    }
}
