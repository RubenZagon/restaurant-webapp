using FluentAssertions;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.ValueObjects;

public class TableIdTests
{
    [Fact]
    public void Constructor_WithValidNumber_ShouldCreateTableId()
    {
        // Arrange
        const int tableNumber = 5;

        // Act
        var tableId = new TableId(tableNumber);

        // Assert
        tableId.Value.Should().Be(tableNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_WithInvalidNumber_ShouldThrowDomainException(int invalidNumber)
    {
        // Act
        var act = () => new TableId(invalidNumber);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*must be positive*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(999)]
    public void Constructor_WithNumberInValidRange_ShouldCreateTableId(int validNumber)
    {
        // Act
        var tableId = new TableId(validNumber);

        // Assert
        tableId.Value.Should().Be(validNumber);
    }

    [Fact]
    public void Equals_WithSameTableId_ShouldReturnTrue()
    {
        // Arrange
        var tableId1 = new TableId(5);
        var tableId2 = new TableId(5);

        // Act & Assert
        tableId1.Should().Be(tableId2);
        (tableId1 == tableId2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentTableId_ShouldReturnFalse()
    {
        // Arrange
        var tableId1 = new TableId(5);
        var tableId2 = new TableId(10);

        // Act & Assert
        tableId1.Should().NotBe(tableId2);
        (tableId1 != tableId2).Should().BeTrue();
    }

    [Fact]
    public void GetHashCode_WithSameTableId_ShouldReturnSameHashCode()
    {
        // Arrange
        var tableId1 = new TableId(5);
        var tableId2 = new TableId(5);

        // Act & Assert
        tableId1.GetHashCode().Should().Be(tableId2.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var tableId = new TableId(5);

        // Act
        var result = tableId.ToString();

        // Assert
        result.Should().Contain("5");
    }
}
