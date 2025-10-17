namespace RestaurantApp.Application.DTOs;

public record OrderDto(
    Guid Id,
    int TableNumber,
    Guid SessionId,
    List<OrderLineDto> Lines,
    string Status,
    decimal Total,
    string Currency,
    DateTime CreatedAt,
    DateTime? ConfirmedAt);

public record OrderLineDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal);

public record AddProductToOrderRequest(
    Guid ProductId,
    int Quantity);
