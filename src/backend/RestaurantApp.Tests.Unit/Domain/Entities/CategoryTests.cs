using FluentAssertions;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        const string name = "Starters";
        const string description = "Delicious appetizers";

        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Id.Should().NotBeNull();
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
        category.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowDomainException(string invalidName)
    {
        // Act
        var act = () => Category.Create(invalidName, "Description");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*name*required*");
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 101);

        // Act
        var act = () => Category.Create(longName, "Description");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*name*cannot exceed 100 characters*");
    }

    [Fact]
    public void Create_WithoutDescription_ShouldCreateCategory()
    {
        // Act
        var category = Category.Create("Starters", null);

        // Assert
        category.Description.Should().BeNull();
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldDeactivateCategory()
    {
        // Arrange
        var category = Category.Create("Starters", "Description");

        // Act
        category.Deactivate();

        // Assert
        category.IsActive.Should().BeFalse();
    }

    [Fact]
    public void Activate_WhenInactive_ShouldActivateCategory()
    {
        // Arrange
        var category = Category.Create("Starters", "Description");
        category.Deactivate();

        // Act
        category.Activate();

        // Assert
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var category = Category.Create("Starters", "Description");
        const string newName = "Appetizers";

        // Act
        category.UpdateName(newName);

        // Assert
        category.Name.Should().Be(newName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithInvalidName_ShouldThrowDomainException(string invalidName)
    {
        // Arrange
        var category = Category.Create("Starters", "Description");

        // Act
        var act = () => category.UpdateName(invalidName);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldUpdateDescription()
    {
        // Arrange
        var category = Category.Create("Starters", "Old description");
        const string newDescription = "New delicious appetizers";

        // Act
        category.UpdateDescription(newDescription);

        // Assert
        category.Description.Should().Be(newDescription);
    }
}
