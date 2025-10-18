using FluentAssertions;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.ValueObjects;

public class PaymentIdTests
{
    [Fact]
    public void Create_WithValidGuid_ShouldCreatePaymentId()
    {
        // Arrange
        var guid = Guid.NewGuid();

        // Act
        var paymentId = new PaymentId(guid);

        // Assert
        paymentId.Value.Should().Be(guid);
    }

    [Fact]
    public void Create_WithEmptyGuid_ShouldThrowDomainException()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        Action act = () => new PaymentId(emptyGuid);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Payment ID cannot be empty");
    }

    [Fact]
    public void Equality_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var paymentId1 = new PaymentId(guid);
        var paymentId2 = new PaymentId(guid);

        // Act & Assert
        paymentId1.Should().Be(paymentId2);
        (paymentId1 == paymentId2).Should().BeTrue();
    }

    [Fact]
    public void Equality_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var paymentId1 = new PaymentId(Guid.NewGuid());
        var paymentId2 = new PaymentId(Guid.NewGuid());

        // Act & Assert
        paymentId1.Should().NotBe(paymentId2);
        (paymentId1 != paymentId2).Should().BeTrue();
    }

    [Fact]
    public void ToString_ShouldReturnGuidAsString()
    {
        // Arrange
        var guid = Guid.NewGuid();
        var paymentId = new PaymentId(guid);

        // Act
        var result = paymentId.ToString();

        // Assert
        result.Should().Be(guid.ToString());
    }
}
