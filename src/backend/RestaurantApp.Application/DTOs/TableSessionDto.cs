namespace RestaurantApp.Application.DTOs;

public record TableSessionDto(
    Guid SessionId,
    int TableNumber,
    DateTime StartedAt);
