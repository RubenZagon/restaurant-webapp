using Microsoft.EntityFrameworkCore;
using RestaurantApp.Application.Ports;
using RestaurantApp.Application.Services;
using RestaurantApp.Application.UseCases;
using RestaurantApp.Infrastructure.Adapters;
using RestaurantApp.Infrastructure.Persistence;
using RestaurantApp.Infrastructure.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL DbContext
var connectionString = builder.Configuration.GetConnectionString("PostgreSQL")
    ?? "Host=localhost;Port=5432;Database=restaurant_db;Username=restaurant_user;Password=restaurant_pass_dev";

builder.Services.AddDbContext<RestaurantDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register application services (Dependency Injection)
builder.Services.AddScoped<ITableRepository, PostgresTableRepository>();
builder.Services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();

// Register payment gateway (mock for development)
builder.Services.AddSingleton<IPaymentGateway>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<MockPaymentGateway>>();
    return new MockPaymentGateway(logger, successRate: 0.9); // 90% success rate
});

// Register SignalR services
builder.Services.AddSignalR();
builder.Services.AddSingleton<IOrderNotificationService, OrderNotificationService>();

builder.Services.AddScoped<StartTableSessionUseCase>();
builder.Services.AddScoped<GetAllCategoriesUseCase>();
builder.Services.AddScoped<GetProductsByCategoryUseCase>();
builder.Services.AddScoped<GetOrCreateOrderForTableUseCase>();
builder.Services.AddScoped<AddProductToOrderUseCase>();
builder.Services.AddScoped<ConfirmOrderUseCase>();
builder.Services.AddScoped<ProcessPaymentUseCase>();
builder.Services.AddScoped<UpdateOrderStatusUseCase>();
builder.Services.AddScoped<GetAllActiveOrdersUseCase>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Apply migrations and seed data automatically on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
    try
    {
        Log.Information("Applying database migrations...");
        dbContext.Database.Migrate();
        Log.Information("Database migrations applied successfully");

        // Seed initial data
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
        var seeder = new DatabaseSeeder(dbContext, logger);
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error applying database migrations or seeding data");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Map SignalR hub
app.MapHub<OrderNotificationHub>("/hubs/order-notifications");

try
{
    Log.Information("Starting Restaurant Application API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Make the implicit Program class public for integration tests
public partial class Program { }
