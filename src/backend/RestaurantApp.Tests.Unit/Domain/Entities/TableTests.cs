using FluentAssertions;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.Entities;

public class TableTests
{
    [Fact]
    public void Constructor_WithValidTableId_ShouldCreateTable()
    {
        // Arrange
        var tableId = new TableId(5);

        // Act
        var table = new Table(tableId);

        // Assert
        table.Id.Should().Be(tableId);
        table.IsOccupied.Should().BeFalse();
        table.ActiveSession.Should().BeNull();
    }

    [Fact]
    public void StartSession_WhenTableFree_ShouldCreateSession()
    {
        // Arrange
        var table = new Table(new TableId(5));

        // Act
        table.StartSession();

        // Assert
        table.IsOccupied.Should().BeTrue();
        table.ActiveSession.Should().NotBeNull();
        table.ActiveSession!.IsActive.Should().BeTrue();
    }

    [Fact]
    public void StartSession_WhenTableOccupied_ShouldKeepExistingSession()
    {
        // Arrange
        var table = new Table(new TableId(5));
        table.StartSession();
        var firstSession = table.ActiveSession;

        // Act
        table.StartSession();

        // Assert
        table.IsOccupied.Should().BeTrue();
        table.ActiveSession.Should().BeSameAs(firstSession);
    }

    [Fact]
    public void EndSession_WhenTableOccupied_ShouldCloseSession()
    {
        // Arrange
        var table = new Table(new TableId(5));
        table.StartSession();

        // Act
        table.EndSession();

        // Assert
        table.IsOccupied.Should().BeFalse();
        table.ActiveSession.Should().BeNull();
    }

    [Fact]
    public void EndSession_WhenTableFree_ShouldThrowDomainException()
    {
        // Arrange
        var table = new Table(new TableId(5));

        // Act
        var act = () => table.EndSession();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*does not have an active session*");
    }
}
