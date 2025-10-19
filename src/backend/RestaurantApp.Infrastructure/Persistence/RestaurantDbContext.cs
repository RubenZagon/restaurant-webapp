using Microsoft.EntityFrameworkCore;
using RestaurantApp.Domain.Entities;

namespace RestaurantApp.Infrastructure.Persistence;

public class RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : DbContext(options)
{
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantDbContext).Assembly);
    }
}
