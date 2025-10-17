using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace RestaurantApp.Infrastructure.Persistence;

public class InMemoryTableRepository : ITableRepository
{
    private readonly ConcurrentDictionary<int, Table> _tables = new();

    public InMemoryTableRepository()
    {
        // Initialize with some tables for development
        for (int i = 1; i <= 20; i++)
        {
            var table = new Table(new TableId(i));
            _tables[i] = table;
        }
    }

    public Task<Table?> GetById(TableId id)
    {
        _tables.TryGetValue(id.Value, out var table);
        return Task.FromResult(table);
    }

    public Task<IEnumerable<Table>> GetAll()
    {
        return Task.FromResult(_tables.Values.AsEnumerable());
    }

    public Task Save(Table table)
    {
        _tables[table.Id.Value] = table;
        return Task.CompletedTask;
    }
}
