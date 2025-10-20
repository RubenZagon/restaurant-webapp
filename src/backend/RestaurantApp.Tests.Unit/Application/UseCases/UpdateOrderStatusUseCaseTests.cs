using FluentAssertions;
using Moq;
using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Application.Services;
using RestaurantApp.Application.UseCases;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Events;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Application.UseCases;

public class UpdateOrderStatusUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IOrderNotificationService> _notificationServiceMock;
    private readonly UpdateOrderStatusUseCase _useCase;

    public UpdateOrderStatusUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _notificationServiceMock = new Mock<IOrderNotificationService>();
        _useCase = new UpdateOrderStatusUseCase(
            _orderRepositoryMock.Object,
            _notificationServiceMock.Object);
    }

    [Fact]
    public async Task Execute_WithValidOrder_AndPreparingStatus_ShouldUpdateToPreparingAndNotify()
    {
        // Arrange
        var orderId = OrderId.Create();
        var order = Order.Create(new TableId(5), SessionId.Create());

        // Add a product and confirm to get to Confirmed status
        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10, "EUR"),
            new Quantity(1));
        order.Confirm();
        order.ClearDomainEvents(); // Clear events from setup

        _orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        var result = await _useCase.Execute(orderId.Value, "Preparing");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Preparing");

        _orderRepositoryMock.Verify(r => r.Save(order), Times.Once);
        _notificationServiceMock.Verify(
            s => s.NotifyOrderStatusChanged(It.Is<OrderStatusChangedEvent>(
                e => e.NewStatus == OrderStatus.Preparing)),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WithValidOrder_AndReadyStatus_ShouldUpdateToReadyAndNotify()
    {
        // Arrange
        var orderId = OrderId.Create();
        var order = Order.Create(new TableId(5), SessionId.Create());

        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10, "EUR"),
            new Quantity(1));
        order.Confirm();
        order.MarkAsPreparing(); // Move to Preparing first
        order.ClearDomainEvents(); // Clear events from previous transitions

        _orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        var result = await _useCase.Execute(orderId.Value, "Ready");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Ready");

        _orderRepositoryMock.Verify(r => r.Save(order), Times.Once);
        _notificationServiceMock.Verify(
            s => s.NotifyOrderStatusChanged(It.Is<OrderStatusChangedEvent>(
                e => e.NewStatus == OrderStatus.Ready)),
            Times.Once);
    }

    [Fact]
    public async Task Execute_WithValidOrder_AndDeliveredStatus_ShouldUpdateToDeliveredAndNotify()
    {
        // Arrange
        var orderId = OrderId.Create();
        var order = Order.Create(new TableId(5), SessionId.Create());

        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10, "EUR"),
            new Quantity(1));
        order.Confirm();
        order.MarkAsPreparing();
        order.MarkAsReady(); // Move to Ready first
        order.ClearDomainEvents();

        _orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        var result = await _useCase.Execute(orderId.Value, "Delivered");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be("Delivered");

        _orderRepositoryMock.Verify(r => r.Save(order), Times.Once);
    }

    [Fact]
    public async Task Execute_WhenOrderNotFound_ShouldReturnFailure()
    {
        // Arrange
        var orderId = OrderId.Create();
        _orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<OrderId>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _useCase.Execute(orderId.Value, "Preparing");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Order not found");
        _orderRepositoryMock.Verify(r => r.Save(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Execute_WithInvalidStatus_ShouldReturnFailure()
    {
        // Arrange
        var orderId = OrderId.Create();
        var order = Order.Create(new TableId(5), SessionId.Create());

        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10, "EUR"),
            new Quantity(1));
        order.Confirm();

        _orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        var result = await _useCase.Execute(orderId.Value, "InvalidStatus");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid status");
    }

    [Fact]
    public async Task Execute_WithInvalidTransition_ShouldReturnFailure()
    {
        // Arrange
        var orderId = OrderId.Create();
        var order = Order.Create(new TableId(5), SessionId.Create());

        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10, "EUR"),
            new Quantity(1));
        order.Confirm();
        // Order is in Confirmed state, trying to mark as Delivered (invalid transition)

        _orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        var result = await _useCase.Execute(orderId.Value, "Delivered");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Only ready orders can be marked as delivered");
    }

    [Fact]
    public async Task Execute_ShouldDispatchDomainEvents()
    {
        // Arrange
        var orderId = OrderId.Create();
        var order = Order.Create(new TableId(5), SessionId.Create());

        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10, "EUR"),
            new Quantity(1));
        order.Confirm();
        order.ClearDomainEvents(); // Clear confirmation events

        _orderRepositoryMock
            .Setup(r => r.GetById(It.IsAny<OrderId>()))
            .ReturnsAsync(order);

        // Act
        await _useCase.Execute(orderId.Value, "Preparing");

        // Assert
        _notificationServiceMock.Verify(
            s => s.NotifyOrderStatusChanged(It.IsAny<OrderStatusChangedEvent>()),
            Times.Once);
    }
}
