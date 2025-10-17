using FluentAssertions;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.ValueObjects;

public class QuantityTests
{
    [Fact]
    public void Constructor_WithValidValue_ShouldCreateQuantity()
    {
        // Arrange
        const int value = 5;

        // Act
        var quantity = new Quantity(value);

        // Assert
        quantity.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidValue_ShouldThrowDomainException(int invalidValue)
    {
        // Act
        var act = () => new Quantity(invalidValue);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*must be at least 1*");
    }

    [Fact]
    public void Constructor_WithMaxValue_ShouldThrowDomainException()
    {
        // Arrange
        const int tooLarge = 101;

        // Act
        var act = () => new Quantity(tooLarge);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 100*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Constructor_WithValidRange_ShouldCreateQuantity(int validValue)
    {
        // Act
        var quantity = new Quantity(validValue);

        // Assert
        quantity.Value.Should().Be(validValue);
    }

    [Fact]
    public void Equals_WithSameQuantity_ShouldReturnTrue()
    {
        // Arrange
        var quantity1 = new Quantity(5);
        var quantity2 = new Quantity(5);

        // Act & Assert
        quantity1.Should().Be(quantity2);
    }

    [Fact]
    public void Add_TwoQuantities_ShouldReturnSum()
    {
        // Arrange
        var quantity1 = new Quantity(5);
        var quantity2 = new Quantity(3);

        // Act
        var result = quantity1.Add(quantity2);

        // Assert
        result.Value.Should().Be(8);
    }

    [Fact]
    public void Add_ExceedingMaximum_ShouldThrowDomainException()
    {
        // Arrange
        var quantity1 = new Quantity(80);
        var quantity2 = new Quantity(30);

        // Act
        var act = () => quantity1.Add(quantity2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot exceed 100*");
    }

    [Fact]
    public void ImplicitConversion_ToInt_ShouldWork()
    {
        // Arrange
        var quantity = new Quantity(5);

        // Act
        int value = quantity;

        // Assert
        value.Should().Be(5);
    }
}
