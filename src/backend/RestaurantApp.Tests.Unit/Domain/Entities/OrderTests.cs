using FluentAssertions;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.Entities;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateOrder()
    {
        // Arrange
        var tableId = new TableId(5);
        var sessionId = SessionId.Create();

        // Act
        var order = Order.Create(tableId, sessionId);

        // Assert
        order.Id.Should().NotBeNull();
        order.TableId.Should().Be(tableId);
        order.SessionId.Should().Be(sessionId);
        order.Status.Should().Be(OrderStatus.Draft);
        order.Lines.Should().BeEmpty();
        order.Total.Amount.Should().Be(0);
    }

    [Fact]
    public void AddProduct_WithValidData_ShouldAddOrderLine()
    {
        // Arrange
        var order = CreateTestOrder();
        var productId = ProductId.Create();
        const string productName = "Paella";
        var price = new Price(18.90m, "EUR");
        var quantity = new Quantity(2);

        // Act
        order.AddProduct(productId, productName, price, quantity);

        // Assert
        order.Lines.Should().HaveCount(1);
        order.Lines.First().ProductId.Should().Be(productId);
        order.Lines.First().Quantity.Value.Should().Be(2);
        order.Total.Amount.Should().Be(37.80m);
    }

    [Fact]
    public void AddProduct_SameProductTwice_ShouldIncreaseQuantity()
    {
        // Arrange
        var order = CreateTestOrder();
        var productId = ProductId.Create();
        const string productName = "Paella";
        var price = new Price(18.90m, "EUR");

        // Act
        order.AddProduct(productId, productName, price, new Quantity(2));
        order.AddProduct(productId, productName, price, new Quantity(1));

        // Assert
        order.Lines.Should().HaveCount(1);
        order.Lines.First().Quantity.Value.Should().Be(3);
        order.Total.Amount.Should().Be(56.70m); // 18.90 * 3
    }

    [Fact]
    public void AddProduct_WhenOrderConfirmed_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateTestOrder();
        // Add a product first so we can confirm the order
        var initialProductId = ProductId.Create();
        var initialPrice = new Price(5m, "EUR");
        var initialQuantity = new Quantity(1);
        order.AddProduct(initialProductId, "Initial Product", initialPrice, initialQuantity);
        order.Confirm();

        var productId = ProductId.Create();
        var price = new Price(10m, "EUR");
        var quantity = new Quantity(1);

        // Act
        var act = () => order.AddProduct(productId, "Product", price, quantity);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot modify*confirmed*");
    }

    [Fact]
    public void RemoveLine_WithValidLineId_ShouldRemoveLine()
    {
        // Arrange
        var order = CreateTestOrder();
        var productId = ProductId.Create();
        order.AddProduct(productId, "Product", new Price(10m, "EUR"), new Quantity(1));
        var lineId = order.Lines.First().Id;

        // Act
        order.RemoveLine(lineId);

        // Assert
        order.Lines.Should().BeEmpty();
        order.Total.Amount.Should().Be(0);
    }

    [Fact]
    public void RemoveLine_WhenOrderConfirmed_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateTestOrder();
        var productId = ProductId.Create();
        order.AddProduct(productId, "Product", new Price(10m, "EUR"), new Quantity(1));
        var lineId = order.Lines.First().Id;
        order.Confirm();

        // Act
        var act = () => order.RemoveLine(lineId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot modify*confirmed*");
    }

    [Fact]
    public void UpdateLineQuantity_WithValidQuantity_ShouldUpdateQuantity()
    {
        // Arrange
        var order = CreateTestOrder();
        var productId = ProductId.Create();
        order.AddProduct(productId, "Product", new Price(10m, "EUR"), new Quantity(2));
        var lineId = order.Lines.First().Id;
        var newQuantity = new Quantity(5);

        // Act
        order.UpdateLineQuantity(lineId, newQuantity);

        // Assert
        order.Lines.First().Quantity.Should().Be(newQuantity);
        order.Total.Amount.Should().Be(50m); // 10 * 5
    }

    [Fact]
    public void Confirm_WhenHasLines_ShouldConfirmOrder()
    {
        // Arrange
        var order = CreateTestOrder();
        order.AddProduct(ProductId.Create(), "Product", new Price(10m, "EUR"), new Quantity(1));

        // Act
        order.Confirm();

        // Assert
        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirm_WhenEmpty_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateTestOrder();

        // Act
        var act = () => order.Confirm();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot confirm*empty*");
    }

    [Fact]
    public void Cancel_WhenDraftOrConfirmed_ShouldCancelOrder()
    {
        // Arrange
        var order = CreateTestOrder();
        order.AddProduct(ProductId.Create(), "Product", new Price(10m, "EUR"), new Quantity(1));

        // Act
        order.Cancel();

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenDelivered_ShouldThrowDomainException()
    {
        // Arrange
        var order = CreateTestOrder();
        order.AddProduct(ProductId.Create(), "Product", new Price(10m, "EUR"), new Quantity(1));
        order.Confirm();
        // Simulate delivery
        typeof(Order).GetProperty("Status")!.SetValue(order, OrderStatus.Delivered);

        // Act
        var act = () => order.Cancel();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*cannot cancel*delivered*");
    }

    [Fact]
    public void Total_ShouldBeSumOfAllLines()
    {
        // Arrange
        var order = CreateTestOrder();

        // Act
        order.AddProduct(ProductId.Create(), "Product1", new Price(10.50m, "EUR"), new Quantity(2));
        order.AddProduct(ProductId.Create(), "Product2", new Price(5.00m, "EUR"), new Quantity(3));

        // Assert
        order.Total.Amount.Should().Be(36.00m); // (10.50 * 2) + (5.00 * 3)
    }

    private static Order CreateTestOrder()
    {
        return Order.Create(new TableId(5), SessionId.Create());
    }
}
