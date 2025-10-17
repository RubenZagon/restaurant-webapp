using FluentAssertions;
using NSubstitute;
using RestaurantApp.Application.Ports;
using RestaurantApp.Application.UseCases;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Application.UseCases;

public class StartTableSessionUseCaseTests
{
    private readonly ITableRepository _tableRepository;
    private readonly StartTableSessionUseCase _useCase;

    public StartTableSessionUseCaseTests()
    {
        _tableRepository = Substitute.For<ITableRepository>();
        _useCase = new StartTableSessionUseCase(_tableRepository);
    }

    [Fact]
    public async Task Execute_WithValidTableId_ShouldStartSession()
    {
        // Arrange
        var tableId = new TableId(5);
        var table = new Table(tableId);
        _tableRepository.GetById(tableId).Returns(table);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.SessionId.Should().NotBeEmpty();
        result.Value.TableNumber.Should().Be(5);
        table.IsOccupied.Should().BeTrue();
        await _tableRepository.Received(1).Save(table);
    }

    [Fact]
    public async Task Execute_WhenTableNotFound_ShouldReturnFailure()
    {
        // Arrange
        var tableId = new TableId(999);
        _tableRepository.GetById(tableId).Returns((Table?)null);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("does not exist");
        await _tableRepository.DidNotReceive().Save(Arg.Any<Table>());
    }

    [Fact]
    public async Task Execute_WhenTableAlreadyOccupied_ShouldReturnFailure()
    {
        // Arrange
        var tableId = new TableId(5);
        var table = new Table(tableId);
        table.StartSession(); // Table already occupied
        _tableRepository.GetById(tableId).Returns(table);

        // Act
        var result = await _useCase.Execute(tableId.Value);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("active session");
    }
}
