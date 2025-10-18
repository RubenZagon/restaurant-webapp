using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.Ports;

/// <summary>
/// Port for payment gateway operations (Hexagonal Architecture)
/// Abstracts payment processing from specific payment providers (Stripe, PayPal, etc.)
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Processes a payment through the payment gateway
    /// </summary>
    /// <param name="request">Payment request details</param>
    /// <returns>Result of the payment processing</returns>
    Task<PaymentResult> ProcessPayment(PaymentRequest request);

    /// <summary>
    /// Checks the status of a payment
    /// </summary>
    /// <param name="paymentId">The payment ID to check</param>
    /// <returns>Current payment status</returns>
    Task<PaymentStatusResult> CheckStatus(PaymentId paymentId);
}

/// <summary>
/// Request model for payment processing
/// </summary>
public record PaymentRequest(
    PaymentId PaymentId,
    Price Amount,
    string PaymentMethod,  // e.g., "credit_card", "debit_card", "paypal"
    Dictionary<string, string>? Metadata = null
);

/// <summary>
/// Result of a payment processing operation
/// </summary>
public record PaymentResult(
    bool Success,
    string? TransactionId = null,
    string? ErrorMessage = null,
    string? ErrorCode = null
);

/// <summary>
/// Result of a payment status check
/// </summary>
public record PaymentStatusResult(
    PaymentStatus Status,
    string? TransactionId = null,
    DateTime? ProcessedAt = null
);
