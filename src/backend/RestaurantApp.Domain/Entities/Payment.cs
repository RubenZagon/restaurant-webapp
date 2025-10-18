using RestaurantApp.Domain.Common;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

/// <summary>
/// Represents a payment for an order
/// </summary>
public class Payment : Entity
{
    public PaymentId Id { get; private set; }
    public OrderId OrderId { get; private set; }
    public Price Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    // Private constructor for EF Core
    private Payment()
    {
        Id = null!;
        OrderId = null!;
        Amount = null!;
    }

    private Payment(
        PaymentId id,
        OrderId orderId,
        Price amount)
    {
        Id = id;
        OrderId = orderId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public static Payment Create(OrderId orderId, Price amount)
    {
        var id = new PaymentId(Guid.NewGuid());
        return new Payment(id, orderId, amount);
    }

    public void MarkAsProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException("Payment can only be marked as processing when in Pending status");

        Status = PaymentStatus.Processing;
    }

    public void MarkAsCompleted(string? transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new DomainException("Transaction ID is required when completing a payment");

        if (Status == PaymentStatus.Completed)
            throw new DomainException("Payment has already been completed");

        Status = PaymentStatus.Completed;
        TransactionId = transactionId;
        ProcessedAt = DateTime.UtcNow;
    }

    public void MarkAsFailed(string? failureReason)
    {
        if (string.IsNullOrWhiteSpace(failureReason))
            throw new DomainException("Failure reason is required when marking payment as failed");

        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == PaymentStatus.Completed)
            throw new DomainException("Cannot cancel a completed payment");

        Status = PaymentStatus.Cancelled;
    }

    public bool IsSuccessful() => Status == PaymentStatus.Completed;
}
