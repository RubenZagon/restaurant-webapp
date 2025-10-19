using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RestaurantApp.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace RestaurantApp.Tests.Integration;

public class WebApplicationFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:15-alpine")
        .WithDatabase("restaurant_db_test")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            services.RemoveAll(typeof(DbContextOptions<RestaurantDbContext>));
            services.RemoveAll(typeof(RestaurantDbContext));

            // Add DbContext with test container connection string
            services.AddDbContext<RestaurantDbContext>(options =>
            {
                options.UseNpgsql(_postgresContainer.GetConnectionString());
            });

            // Build service provider and apply migrations
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();

            // Apply migrations
            dbContext.Database.Migrate();
        });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }
}
