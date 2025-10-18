namespace RestaurantApp.Application.DTOs;

/// <summary>
/// Data Transfer Object for Payment
/// </summary>
public record PaymentDto(
    string Id,
    string OrderId,
    decimal Amount,
    string Currency,
    string Status,
    string? TransactionId,
    string? FailureReason,
    DateTime CreatedAt,
    DateTime? ProcessedAt
);
