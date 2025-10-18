using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

public class ConfirmOrderUseCase
{
    private readonly IOrderRepository _orderRepository;

    public ConfirmOrderUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
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

            var dto = MapToDto(order);
            return Result<OrderDto>.Success(dto);
        }
        catch (DomainException ex)
        {
            return Result<OrderDto>.Failure(ex.Message);
        }
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
