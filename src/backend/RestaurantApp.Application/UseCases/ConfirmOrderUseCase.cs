using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Application.Services;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Events;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

public class ConfirmOrderUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderNotificationService _notificationService;

    public ConfirmOrderUseCase(
        IOrderRepository orderRepository,
        IOrderNotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _notificationService = notificationService;
    }

    public async Task<Result<OrderDto>> Execute(Guid orderId)
    {
        try
        {
            var order = await _orderRepository.GetById(OrderId.From(orderId));
            if (order == null)
            {
                return Result<OrderDto>.Failure($"Order with ID {orderId} not found.");
            }

            order.Confirm();
            await _orderRepository.Save(order);

            // Dispatch domain events
            await DispatchDomainEvents(order);

            var dto = MapToDto(order);
            return Result<OrderDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<OrderDto>.Failure(ex.Message);
        }
    }

    private async Task DispatchDomainEvents(Order order)
    {
        foreach (var domainEvent in order.DomainEvents)
        {
            switch (domainEvent)
            {
                case OrderConfirmedEvent confirmedEvent:
                    await _notificationService.NotifyOrderConfirmed(confirmedEvent);
                    break;
                case OrderStatusChangedEvent statusChangedEvent:
                    await _notificationService.NotifyOrderStatusChanged(statusChangedEvent);
                    break;
            }
        }

        order.ClearDomainEvents();
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto(
            order.Id.Value,
            order.TableId.Value,
            order.SessionId.Value,
            order.Lines.Select(l => new OrderLineDto(
                l.Id.Value,
                l.ProductId.Value,
                l.ProductName,
                l.UnitPrice.Amount,
                l.Quantity.Value,
                l.Subtotal.Amount)).ToList(),
            order.Status.ToString(),
            order.Total.Amount,
            order.Total.Currency,
            order.CreatedAt,
            order.ConfirmedAt);
    }
}
