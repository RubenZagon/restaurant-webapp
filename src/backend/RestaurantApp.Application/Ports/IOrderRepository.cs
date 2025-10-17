using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.Ports;

public interface IOrderRepository
{
    Task<Order?> GetById(OrderId id);
    Task<Order?> GetActiveOrderByTable(TableId tableId);
    Task<IEnumerable<Order>> GetAll();
    Task Save(Order order);
    Task Delete(OrderId id);
}
