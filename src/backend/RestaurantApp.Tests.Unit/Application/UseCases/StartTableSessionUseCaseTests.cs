using FluentAssertions;
using Moq;
using RestaurantApp.Application.Ports;
using RestaurantApp.Application.UseCases;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Application.UseCases;

public class StartTableSessionUseCaseTests
{
    private readonly Mock<ITableRepository> _tableRepositoryMock;
    private readonly StartTableSessionUseCase _useCase;

    public StartTableSessionUseCaseTests()
    {
        _tableRepositoryMock = new Mock<ITableRepository>();
        _useCase = new StartTableSessionUseCase(_tableRepositoryMock.Object);
    }

    [Fact]
    public async Task Execute_WithValidTableId_ShouldStartSession()
    {
        // Arrange
        var tableId = new TableId(5);
        var table = new Table(tableId);
        _tableRepositoryMock.Setup(r => r.GetById(tableId)).ReturnsAsync(table);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.SessionId.Should().NotBeEmpty();
        result.Value.TableNumber.Should().Be(5);
        table.IsOccupied.Should().BeTrue();
        _tableRepositoryMock.Verify(r => r.Save(table), Times.Once);
    }

    [Fact]
    public async Task Execute_WhenTableNotFound_ShouldReturnFailure()
    {
        // Arrange
        var tableId = new TableId(999);
        _tableRepositoryMock.Setup(r => r.GetById(tableId)).ReturnsAsync((Table?)null);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("does not exist");
        _tableRepositoryMock.Verify(r => r.Save(It.IsAny<Table>()), Times.Never);
    }

    [Fact]
    public async Task Execute_WhenTableAlreadyOccupied_ShouldReturnFailure()
    {
        // Arrange
        var tableId = new TableId(5);
        var table = new Table(tableId);
        table.StartSession(); // Table already occupied
        _tableRepositoryMock.Setup(r => r.GetById(tableId)).ReturnsAsync(table);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("active session");
    }
}
