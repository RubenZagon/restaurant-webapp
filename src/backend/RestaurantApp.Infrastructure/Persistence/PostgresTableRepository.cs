using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence;

public class PostgresTableRepository : ITableRepository
{
    private readonly RestaurantDbContext _context;

    public PostgresTableRepository(RestaurantDbContext context)
    {
        _context = context;
    }

    public async Task<Table?> GetById(TableId id)
    {
        return await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Table>> GetAll()
    {
        return await _context.Tables.ToListAsync();
    }

    public async Task Save(Table table)
    {
        var existingTable = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == table.Id);

        if (existingTable == null)
        {
            await _context.Tables.AddAsync(table);
        }
        else
        {
            // Update the existing table entity
            // Use Update instead of SetValues to properly handle owned entities
            _context.Entry(existingTable).State = EntityState.Detached;
            _context.Tables.Update(table);
        }

        await _context.SaveChangesAsync();
    }
}
