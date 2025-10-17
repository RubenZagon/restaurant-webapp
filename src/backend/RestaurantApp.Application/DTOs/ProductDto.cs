namespace RestaurantApp.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    string Currency,
    Guid CategoryId,
    IEnumerable<string> Allergens,
    bool IsAvailable);
