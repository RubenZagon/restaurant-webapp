using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RestaurantApp.Infrastructure.Persistence;

/// <summary>
/// Factory for creating DbContext instances at design time (for migrations)
/// </summary>
public class RestaurantDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
{
    public RestaurantDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<RestaurantDbContext>();

        // Use a default connection string for migrations
        // This will be overridden at runtime by the actual configuration
        optionsBuilder.UseNpgsql(
            "Host=localhost;Port=5432;Database=restaurant_db;Username=restaurant_user;Password=restaurant_pass_dev");

        return new RestaurantDbContext(optionsBuilder.Options);
    }
}
