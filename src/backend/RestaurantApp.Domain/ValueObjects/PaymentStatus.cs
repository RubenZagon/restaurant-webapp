namespace RestaurantApp.Domain.ValueObjects;

/// <summary>
/// Represents the status of a payment in the payment lifecycle
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Payment has been initiated but not yet processed
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Payment is being processed by the payment gateway
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Payment was successfully completed
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Payment failed due to insufficient funds, card declined, etc.
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Payment was cancelled by the user or system
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Payment was refunded
    /// </summary>
    Refunded = 5
}
