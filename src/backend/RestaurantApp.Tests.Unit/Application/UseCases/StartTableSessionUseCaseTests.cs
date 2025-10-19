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
    public async Task Execute_WhenTableNotFound_ShouldCreateTableAndStartSession()
    {
        // Arrange
        var tableId = new TableId(999);
        _tableRepositoryMock.Setup(r => r.GetById(tableId)).ReturnsAsync((Table?)null);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.SessionId.Should().NotBeEmpty();
        result.Value.TableNumber.Should().Be(999);
        // Should save twice: once to create the table, once to start the session
        _tableRepositoryMock.Verify(r => r.Save(It.IsAny<Table>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Execute_WhenTableAlreadyOccupied_ShouldReturnExistingSession()
    {
        // Arrange
        var tableId = new TableId(5);
        var table = new Table(tableId);
        table.StartSession(); // Table already occupied
        var existingSessionId = table.ActiveSession!.Id.Value;
        _tableRepositoryMock.Setup(r => r.GetById(tableId)).ReturnsAsync(table);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.SessionId.Should().Be(existingSessionId); // Should return the same session
        result.Value.TableNumber.Should().Be(5);
        // Should not save again since session already exists
        _tableRepositoryMock.Verify(r => r.Save(It.IsAny<Table>()), Times.Never);
    }
}
