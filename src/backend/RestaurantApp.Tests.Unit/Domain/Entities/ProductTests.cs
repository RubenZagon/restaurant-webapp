using FluentAssertions;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        const string name = "Paella";
        const string description = "Traditional Spanish rice dish";
        var price = new Price(15.99m, "EUR");
        var categoryId = CategoryId.Create();
        var allergens = new Allergens(new List<string> { "Shellfish" });

        // Act
        var product = Product.Create(name, description, price, categoryId, allergens);

        // Assert
        product.Id.Should().NotBeNull();
        product.Name.Should().Be(name);
        product.Description.Should().Be(description);
        product.Price.Should().Be(price);
        product.CategoryId.Should().Be(categoryId);
        product.Allergens.Should().Be(allergens);
        product.IsAvailable.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrowDomainException(string invalidName)
    {
        // Arrange
        var price = new Price(10m, "EUR");
        var categoryId = CategoryId.Create();

        // Act
        var act = () => Product.Create(invalidName, "Description", price, categoryId, Allergens.None());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*name*required*");
    }

    [Fact]
    public void Create_WithNameTooLong_ShouldThrowDomainException()
    {
        // Arrange
        var longName = new string('A', 201);
        var price = new Price(10m, "EUR");
        var categoryId = CategoryId.Create();

        // Act
        var act = () => Product.Create(longName, "Description", price, categoryId, Allergens.None());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*name*cannot exceed 200 characters*");
    }

    [Fact]
    public void Create_WithoutAllergens_ShouldCreateProduct()
    {
        // Arrange
        const string name = "Water";
        var price = new Price(1.50m, "EUR");
        var categoryId = CategoryId.Create();

        // Act
        var product = Product.Create(name, "Mineral water", price, categoryId, Allergens.None());

        // Assert
        product.Allergens.HasAllergens.Should().BeFalse();
    }

    [Fact]
    public void MakeUnavailable_WhenAvailable_ShouldMakeUnavailable()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        product.MakeUnavailable();

        // Assert
        product.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public void MakeAvailable_WhenUnavailable_ShouldMakeAvailable()
    {
        // Arrange
        var product = CreateTestProduct();
        product.MakeUnavailable();

        // Act
        product.MakeAvailable();

        // Assert
        product.IsAvailable.Should().BeTrue();
    }

    [Fact]
    public void UpdatePrice_WithValidPrice_ShouldUpdatePrice()
    {
        // Arrange
        var product = CreateTestProduct();
        var newPrice = new Price(20.99m, "EUR");

        // Act
        product.UpdatePrice(newPrice);

        // Assert
        product.Price.Should().Be(newPrice);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var product = CreateTestProduct();
        const string newName = "Updated Paella";

        // Act
        product.UpdateName(newName);

        // Assert
        product.Name.Should().Be(newName);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        // Arrange
        var product = CreateTestProduct();
        const string newDescription = "New amazing description";

        // Act
        product.UpdateDescription(newDescription);

        // Assert
        product.Description.Should().Be(newDescription);
    }

    [Fact]
    public void UpdateCategory_WithValidCategoryId_ShouldUpdateCategory()
    {
        // Arrange
        var product = CreateTestProduct();
        var newCategoryId = CategoryId.Create();

        // Act
        product.UpdateCategory(newCategoryId);

        // Assert
        product.CategoryId.Should().Be(newCategoryId);
    }

    private static Product CreateTestProduct()
    {
        return Product.Create(
            "Paella",
            "Traditional Spanish rice dish",
            new Price(15.99m, "EUR"),
            CategoryId.Create(),
            new Allergens(new List<string> { "Shellfish" })
        );
    }
}
