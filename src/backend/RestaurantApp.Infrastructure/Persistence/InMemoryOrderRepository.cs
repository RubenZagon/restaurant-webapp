using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace RestaurantApp.Infrastructure.Persistence;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task<Order?> GetById(OrderId id)
    {
        _orders.TryGetValue(id.Value, out var order);
        return Task.FromResult(order);
    }

    public Task<Order?> GetActiveOrderByTable(TableId tableId)
    {
        var order = _orders.Values
            .FirstOrDefault(o =>
                o.TableId.Value == tableId.Value &&
                (o.Status == OrderStatus.Draft || o.Status == OrderStatus.Confirmed));

        return Task.FromResult(order);
    }

    public Task<IEnumerable<Order>> GetAll()
    {
        return Task.FromResult(_orders.Values.AsEnumerable());
    }

    public Task Save(Order order)
    {
        _orders[order.Id.Value] = order;
        return Task.CompletedTask;
    }

    public Task Delete(OrderId id)
    {
        _orders.TryRemove(id.Value, out _);
        return Task.CompletedTask;
    }
}
