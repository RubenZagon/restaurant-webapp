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

// Register application services (Dependency Injection)
builder.Services.AddSingleton<ITableRepository, InMemoryTableRepository>();
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
