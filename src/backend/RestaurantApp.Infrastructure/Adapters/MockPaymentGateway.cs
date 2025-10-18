using Microsoft.Extensions.Logging;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Adapters;

/// <summary>
/// Mock implementation of payment gateway for development and testing
/// Simulates payment processing without actual charges
/// </summary>
public class MockPaymentGateway : IPaymentGateway
{
    private readonly ILogger<MockPaymentGateway> _logger;
    private readonly Dictionary<Guid, (PaymentStatus Status, string? TransactionId, DateTime? ProcessedAt)> _payments = new();

    // Configurable success rate for testing failure scenarios
    private readonly double _successRate;

    public MockPaymentGateway(ILogger<MockPaymentGateway> logger, double successRate = 1.0)
    {
        _logger = logger;
        _successRate = Math.Clamp(successRate, 0.0, 1.0);
    }

    public async Task<PaymentResult> ProcessPayment(PaymentRequest request)
    {
        _logger.LogInformation(
            "Processing mock payment. PaymentId: {PaymentId}, Amount: {Amount}, Method: {Method}",
            request.PaymentId,
            request.Amount,
            request.PaymentMethod);

        // Simulate network delay
        await Task.Delay(Random.Shared.Next(100, 500));

        // Simulate success/failure based on configured success rate
        var random = Random.Shared.NextDouble();
        var isSuccess = random <= _successRate;

        if (isSuccess)
        {
            var transactionId = $"txn_mock_{Guid.NewGuid().ToString()[..8]}";

            // Store payment status
            _payments[request.PaymentId.Value] = (PaymentStatus.Completed, transactionId, DateTime.UtcNow);

            _logger.LogInformation(
                "Mock payment successful. PaymentId: {PaymentId}, TransactionId: {TransactionId}",
                request.PaymentId,
                transactionId);

            return new PaymentResult(
                Success: true,
                TransactionId: transactionId
            );
        }
        else
        {
            // Store failed payment status
            _payments[request.PaymentId.Value] = (PaymentStatus.Failed, null, DateTime.UtcNow);

            var errorCode = GetRandomErrorCode();
            var errorMessage = GetErrorMessage(errorCode);

            _logger.LogWarning(
                "Mock payment failed. PaymentId: {PaymentId}, ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                request.PaymentId,
                errorCode,
                errorMessage);

            return new PaymentResult(
                Success: false,
                ErrorCode: errorCode,
                ErrorMessage: errorMessage
            );
        }
    }

    public async Task<PaymentStatusResult> CheckStatus(PaymentId paymentId)
    {
        _logger.LogInformation("Checking mock payment status. PaymentId: {PaymentId}", paymentId);

        // Simulate network delay
        await Task.Delay(Random.Shared.Next(50, 200));

        if (_payments.TryGetValue(paymentId.Value, out var paymentInfo))
        {
            return new PaymentStatusResult(
                Status: paymentInfo.Status,
                TransactionId: paymentInfo.TransactionId,
                ProcessedAt: paymentInfo.ProcessedAt
            );
        }

        // Payment not found, assume it's still pending
        return new PaymentStatusResult(
            Status: PaymentStatus.Pending
        );
    }

    private static string GetRandomErrorCode()
    {
        var errorCodes = new[]
        {
            "insufficient_funds",
            "card_declined",
            "expired_card",
            "invalid_card",
            "processing_error",
            "network_timeout"
        };

        return errorCodes[Random.Shared.Next(errorCodes.Length)];
    }

    private static string GetErrorMessage(string errorCode)
    {
        return errorCode switch
        {
            "insufficient_funds" => "Insufficient funds in the account",
            "card_declined" => "Card was declined by the issuer",
            "expired_card" => "Card has expired",
            "invalid_card" => "Invalid card number",
            "processing_error" => "Error processing the payment",
            "network_timeout" => "Network timeout while processing payment",
            _ => "Unknown error occurred"
        };
    }
}
