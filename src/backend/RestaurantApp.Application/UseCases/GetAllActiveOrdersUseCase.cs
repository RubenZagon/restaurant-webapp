using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

/// <summary>
/// Use case for fetching all active orders for the kitchen dashboard.
/// Returns orders that are Confirmed, Preparing, Ready, or Delivered.
/// Excludes Draft and Cancelled orders.
/// </summary>
public class GetAllActiveOrdersUseCase
{
    private readonly IOrderRepository _orderRepository;

    public GetAllActiveOrdersUseCase(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<IEnumerable<OrderDto>>> Execute()
    {
        var allOrders = await _orderRepository.GetAll();

        // Filter to only include active orders (exclude Draft and Cancelled)
        var activeOrders = allOrders
            .Where(o => o.Status != OrderStatus.Draft && o.Status != OrderStatus.Cancelled)
            .OrderBy(o => o.CreatedAt)
            .ToList();

        var orderDtos = activeOrders.Select(order => new OrderDto(
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
        )).ToList();

        return Result<IEnumerable<OrderDto>>.Success(orderDtos);
    }
}
