using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.Ports;

public interface ITableRepository
{
    Task<Table?> GetById(TableId id);
    Task<IEnumerable<Table>> GetAll();
    Task Save(Table table);
}
