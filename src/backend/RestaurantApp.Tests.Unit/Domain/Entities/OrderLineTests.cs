using FluentAssertions;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.Entities;

public class OrderLineTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrderLine()
    {
        // Arrange
        var productId = ProductId.Create();
        const string productName = "Paella";
        var price = new Price(18.90m, "EUR");
        var quantity = new Quantity(2);

        // Act
        var orderLine = OrderLine.Create(productId, productName, price, quantity);

        // Assert
        orderLine.Id.Should().NotBeNull();
        orderLine.ProductId.Should().Be(productId);
        orderLine.ProductName.Should().Be(productName);
        orderLine.UnitPrice.Should().Be(price);
        orderLine.Quantity.Should().Be(quantity);
        orderLine.Subtotal.Amount.Should().Be(37.80m); // 18.90 * 2
    }

    [Fact]
    public void UpdateQuantity_WithValidQuantity_ShouldUpdateAndRecalculate()
    {
        // Arrange
        var orderLine = CreateTestOrderLine();
        var newQuantity = new Quantity(5);

        // Act
        orderLine.UpdateQuantity(newQuantity);

        // Assert
        orderLine.Quantity.Should().Be(newQuantity);
        orderLine.Subtotal.Amount.Should().Be(94.50m); // 18.90 * 5
    }

    [Fact]
    public void Subtotal_ShouldBeCalculatedCorrectly()
    {
        // Arrange
        var price = new Price(10.50m, "EUR");
        var quantity = new Quantity(3);
        var productId = ProductId.Create();

        // Act
        var orderLine = OrderLine.Create(productId, "Test Product", price, quantity);

        // Assert
        orderLine.Subtotal.Amount.Should().Be(31.50m); // 10.50 * 3
        orderLine.Subtotal.Currency.Should().Be("EUR");
    }

    private static OrderLine CreateTestOrderLine()
    {
        return OrderLine.Create(
            ProductId.Create(),
            "Paella",
            new Price(18.90m, "EUR"),
            new Quantity(2));
    }
}
