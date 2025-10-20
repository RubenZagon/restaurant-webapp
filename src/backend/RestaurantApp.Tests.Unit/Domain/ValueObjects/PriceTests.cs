using FluentAssertions;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.ValueObjects;

public class PriceTests
{
    [Fact]
    public void Constructor_WithValidAmount_ShouldCreatePrice()
    {
        // Arrange
        const decimal amount = 15.99m;
        const string currency = "EUR";

        // Act
        var price = new Price(amount, currency);

        // Assert
        price.Amount.Should().Be(amount);
        price.Currency.Should().Be(currency);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public void Constructor_WithNegativeAmount_ShouldThrowDomainException(decimal invalidAmount)
    {
        // Act
        var act = () => new Price(invalidAmount, "EUR");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot be negative*");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidCurrency_ShouldThrowDomainException(string invalidCurrency)
    {
        // Act
        var act = () => new Price(10.0m, invalidCurrency);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*Currency*required*");
    }

    [Theory]
    [InlineData("EURO")]
    [InlineData("US")]
    [InlineData("ABCD")]
    public void Constructor_WithInvalidCurrencyCode_ShouldThrowDomainException(string invalidCode)
    {
        // Act
        var act = () => new Price(10.0m, invalidCode);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*must be a valid 3-letter*");
    }

    [Theory]
    [InlineData("EUR")]
    [InlineData("USD")]
    [InlineData("GBP")]
    public void Constructor_WithValidCurrencyCode_ShouldCreatePrice(string validCode)
    {
        // Act
        var price = new Price(10.0m, validCode);

        // Assert
        price.Currency.Should().Be(validCode.ToUpper());
    }

    [Fact]
    public void Equals_WithSamePrice_ShouldReturnTrue()
    {
        // Arrange
        var price1 = new Price(15.99m, "EUR");
        var price2 = new Price(15.99m, "EUR");

        // Act & Assert
        price1.Should().Be(price2);
        (price1 == price2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldReturnFalse()
    {
        // Arrange
        var price1 = new Price(15.99m, "EUR");
        var price2 = new Price(20.00m, "EUR");

        // Act & Assert
        price1.Should().NotBe(price2);
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldReturnFalse()
    {
        // Arrange
        var price1 = new Price(15.99m, "EUR");
        var price2 = new Price(15.99m, "USD");

        // Act & Assert
        price1.Should().NotBe(price2);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedPrice()
    {
        // Arrange
        var price = new Price(15.99m, "EUR");

        // Act
        var result = price.ToString();

        // Assert
        result.Should().Contain("15,99");
        result.Should().Contain("EUR");
    }

    [Fact]
    public void Add_TwoPricesWithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        var price1 = new Price(10.50m, "EUR");
        var price2 = new Price(5.25m, "EUR");

        // Act
        var result = price1.Add(price2);

        // Assert
        result.Amount.Should().Be(15.75m);
        result.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Add_TwoPricesWithDifferentCurrency_ShouldThrowDomainException()
    {
        // Arrange
        var price1 = new Price(10.50m, "EUR");
        var price2 = new Price(5.25m, "USD");

        // Act
        var act = () => price1.Add(price2);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*same currency*");
    }

    [Fact]
    public void Multiply_ByPositiveQuantity_ShouldReturnMultipliedPrice()
    {
        // Arrange
        var price = new Price(10.50m, "EUR");
        const int quantity = 3;

        // Act
        var result = price.Multiply(quantity);

        // Assert
        result.Amount.Should().Be(31.50m);
        result.Currency.Should().Be("EUR");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Multiply_ByInvalidQuantity_ShouldThrowDomainException(int invalidQuantity)
    {
        // Arrange
        var price = new Price(10.50m, "EUR");

        // Act
        var act = () => price.Multiply(invalidQuantity);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*must be positive*");
    }
}
