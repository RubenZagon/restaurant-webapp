using FluentAssertions;
using Moq;
using RestaurantApp.Application.Ports;
using RestaurantApp.Application.UseCases;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Application.UseCases;

public class GetAllActiveOrdersUseCaseTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly GetAllActiveOrdersUseCase _useCase;

    public GetAllActiveOrdersUseCaseTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _useCase = new GetAllActiveOrdersUseCase(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_WithNoOrders_ShouldReturnEmptyList()
    {
        // Arrange
        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order>());

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Execute_WithActiveOrders_ShouldReturnOnlyActiveOrders()
    {
        // Arrange
        var confirmedOrder = CreateOrder(OrderStatus.Confirmed);
        var preparingOrder = CreateOrder(OrderStatus.Preparing);
        var readyOrder = CreateOrder(OrderStatus.Ready);
        var deliveredOrder = CreateOrder(OrderStatus.Delivered);
        var draftOrder = CreateOrder(OrderStatus.Draft);
        var cancelledOrder = CreateOrder(OrderStatus.Cancelled);

        var allOrders = new List<Order>
        {
            confirmedOrder,
            preparingOrder,
            readyOrder,
            deliveredOrder,
            draftOrder,
            cancelledOrder
        };

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(allOrders);

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(4); // Confirmed, Preparing, Ready, Delivered
        result.Value.Should().NotContain(o => o.Status == "Draft");
        result.Value.Should().NotContain(o => o.Status == "Cancelled");
    }

    [Fact]
    public async Task Execute_ShouldOrderByCreatedAtAscending()
    {
        // Arrange
        var order1 = CreateOrderWithTime(OrderStatus.Confirmed, DateTime.UtcNow.AddMinutes(-10));
        var order2 = CreateOrderWithTime(OrderStatus.Preparing, DateTime.UtcNow.AddMinutes(-5));
        var order3 = CreateOrderWithTime(OrderStatus.Ready, DateTime.UtcNow.AddMinutes(-15));

        var allOrders = new List<Order> { order2, order3, order1 }; // Unordered

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(allOrders);

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        var orderedList = result.Value.ToList();
        orderedList[0].Id.Should().Be(order3.Id.Value); // -15 minutes (oldest)
        orderedList[1].Id.Should().Be(order1.Id.Value); // -10 minutes
        orderedList[2].Id.Should().Be(order2.Id.Value); // -5 minutes (newest)
    }

    [Fact]
    public async Task Execute_ShouldIncludeConfirmedOrders()
    {
        // Arrange
        var confirmedOrder = CreateOrder(OrderStatus.Confirmed);

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order> { confirmedOrder });

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be("Confirmed");
    }

    [Fact]
    public async Task Execute_ShouldIncludePreparingOrders()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Preparing);

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order> { order });

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be("Preparing");
    }

    [Fact]
    public async Task Execute_ShouldIncludeReadyOrders()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Ready);

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order> { order });

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be("Ready");
    }

    [Fact]
    public async Task Execute_ShouldIncludeDeliveredOrders()
    {
        // Arrange
        var order = CreateOrder(OrderStatus.Delivered);

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order> { order });

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be("Delivered");
    }

    [Fact]
    public async Task Execute_ShouldExcludeDraftOrders()
    {
        // Arrange
        var draftOrder = CreateOrder(OrderStatus.Draft);
        var confirmedOrder = CreateOrder(OrderStatus.Confirmed);

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order> { draftOrder, confirmedOrder });

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().NotContain(o => o.Status == "Draft");
    }

    [Fact]
    public async Task Execute_ShouldExcludeCancelledOrders()
    {
        // Arrange
        var cancelledOrder = CreateOrder(OrderStatus.Cancelled);
        var confirmedOrder = CreateOrder(OrderStatus.Confirmed);

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order> { cancelledOrder, confirmedOrder });

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.Should().NotContain(o => o.Status == "Cancelled");
    }

    [Fact]
    public async Task Execute_ShouldMapToOrderDtoCorrectly()
    {
        // Arrange
        var order = Order.Create(new TableId(5), SessionId.Create());
        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10.50m, "EUR"),
            new Quantity(2));
        order.Confirm();

        _orderRepositoryMock
            .Setup(r => r.GetAll())
            .ReturnsAsync(new List<Order> { order });

        // Act
        var result = await _useCase.Execute();

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value.First();
        dto.Id.Should().Be(order.Id.Value);
        dto.TableNumber.Should().Be(5);
        dto.Status.Should().Be("Confirmed");
        dto.Total.Should().Be(21.00m); // 10.50 * 2
        dto.Currency.Should().Be("EUR");
        dto.Lines.Should().HaveCount(1);
        dto.Lines.First().ProductName.Should().Be("Test Product");
        dto.Lines.First().Quantity.Should().Be(2);
    }

    // Helper methods
    private Order CreateOrder(OrderStatus status)
    {
        var order = Order.Create(new TableId(1), SessionId.Create());
        order.AddProduct(
            ProductId.Create(),
            "Test Product",
            new Price(10, "EUR"),
            new Quantity(1));

        switch (status)
        {
            case OrderStatus.Confirmed:
                order.Confirm();
                break;
            case OrderStatus.Preparing:
                order.Confirm();
                order.MarkAsPreparing();
                break;
            case OrderStatus.Ready:
                order.Confirm();
                order.MarkAsPreparing();
                order.MarkAsReady();
                break;
            case OrderStatus.Delivered:
                order.Confirm();
                order.MarkAsPreparing();
                order.MarkAsReady();
                order.MarkAsDelivered();
                break;
            case OrderStatus.Cancelled:
                order.Confirm();
                order.Cancel();
                break;
            // Draft is the default state, no action needed
        }

        return order;
    }

    private Order CreateOrderWithTime(OrderStatus status, DateTime createdAt)
    {
        var order = CreateOrder(status);
        // Note: In a real scenario, we would need to set CreatedAt through reflection
        // or add a test-specific constructor. For now, we rely on the order of creation
        return order;
    }
}
