using FluentAssertions;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.Entities;

public class PaymentTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePayment()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");

        // Act
        var payment = Payment.Create(orderId, amount);

        // Assert
        payment.Id.Should().NotBe(null);
        payment.OrderId.Should().Be(orderId);
        payment.Amount.Should().Be(amount);
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        payment.ProcessedAt.Should().BeNull();
        payment.FailureReason.Should().BeNull();
    }

    [Fact]
    public void MarkAsProcessing_WhenPending_ShouldUpdateStatus()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);

        // Act
        payment.MarkAsProcessing();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Processing);
    }

    [Fact]
    public void MarkAsProcessing_WhenNotPending_ShouldThrowException()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);
        payment.MarkAsCompleted("txn_123456");

        // Act
        Action act = () => payment.MarkAsProcessing();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Payment can only be marked as processing when in Pending status");
    }

    [Fact]
    public void MarkAsCompleted_WithValidData_ShouldUpdateStatusAndTimestamp()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);
        payment.MarkAsProcessing();
        var transactionId = "txn_abc123";

        // Act
        payment.MarkAsCompleted(transactionId);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Completed);
        payment.TransactionId.Should().Be(transactionId);
        payment.ProcessedAt.Should().NotBeNull();
        payment.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsCompleted_WithNullTransactionId_ShouldThrowException()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);

        // Act
        Action act = () => payment.MarkAsCompleted(null);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Transaction ID is required when completing a payment");
    }

    [Fact]
    public void MarkAsCompleted_WhenAlreadyCompleted_ShouldThrowException()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);
        payment.MarkAsCompleted("txn_123");

        // Act
        Action act = () => payment.MarkAsCompleted("txn_456");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Payment has already been completed");
    }

    [Fact]
    public void MarkAsFailed_WithReason_ShouldUpdateStatusAndReason()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);
        payment.MarkAsProcessing();
        var failureReason = "Insufficient funds";

        // Act
        payment.MarkAsFailed(failureReason);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.FailureReason.Should().Be(failureReason);
        payment.ProcessedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsFailed_WithNullReason_ShouldThrowException()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);

        // Act
        Action act = () => payment.MarkAsFailed(null);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Failure reason is required when marking payment as failed");
    }

    [Fact]
    public void Cancel_WhenPendingOrProcessing_ShouldUpdateStatus()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);

        // Act
        payment.Cancel();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenCompleted_ShouldThrowException()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);
        payment.MarkAsCompleted("txn_123");

        // Act
        Action act = () => payment.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel a completed payment");
    }

    [Fact]
    public void IsSuccessful_WhenCompleted_ShouldReturnTrue()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);
        payment.MarkAsCompleted("txn_123");

        // Act
        var result = payment.IsSuccessful();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsSuccessful_WhenNotCompleted_ShouldReturnFalse()
    {
        // Arrange
        var orderId = OrderId.From(Guid.NewGuid());
        var amount = new Price(25.50m, "EUR");
        var payment = Payment.Create(orderId, amount);

        // Act
        var result = payment.IsSuccessful();

        // Assert
        result.Should().BeFalse();
    }
}
