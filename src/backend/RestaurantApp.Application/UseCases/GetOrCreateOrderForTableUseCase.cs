using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

public class GetOrCreateOrderForTableUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ITableRepository _tableRepository;

    public GetOrCreateOrderForTableUseCase(
        IOrderRepository orderRepository,
        ITableRepository tableRepository)
    {
        _orderRepository = orderRepository;
        _tableRepository = tableRepository;
    }

    public async Task<Result<OrderDto>> Execute(int tableNumber)
    {
        var tableId = new TableId(tableNumber);
        var table = await _tableRepository.GetById(tableId);

        if (table == null)
        {
            return Result<OrderDto>.Failure($"Table {tableNumber} does not exist.");
        }

        if (!table.IsOccupied)
        {
            return Result<OrderDto>.Failure($"Table {tableNumber} does not have an active session.");
        }

        var existingOrder = await _orderRepository.GetActiveOrderByTable(tableId);

        Order order;
        if (existingOrder == null)
        {
            order = Order.Create(tableId, table.ActiveSession!.Id);
            await _orderRepository.Save(order);
        }
        else
        {
            order = existingOrder;
        }

        var dto = MapToDto(order, tableNumber);
        return Result<OrderDto>.Success(dto);
    }

    private static OrderDto MapToDto(Order order, int tableNumber)
    {
        return new OrderDto(
            order.Id.Value,
            tableNumber,
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
