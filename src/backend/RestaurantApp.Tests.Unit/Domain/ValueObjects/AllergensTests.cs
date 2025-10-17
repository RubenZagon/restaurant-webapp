using FluentAssertions;
using RestaurantApp.Domain.ValueObjects;
using Xunit;

namespace RestaurantApp.Tests.Unit.Domain.ValueObjects;

public class AllergensTests
{
    [Fact]
    public void Constructor_WithValidAllergens_ShouldCreateAllergens()
    {
        // Arrange
        var allergenList = new List<string> { "Gluten", "Dairy", "Nuts" };

        // Act
        var allergens = new Allergens(allergenList);

        // Assert
        allergens.Values.Should().HaveCount(3);
        allergens.Values.Should().Contain("Gluten");
        allergens.Values.Should().Contain("Dairy");
        allergens.Values.Should().Contain("Nuts");
    }

    [Fact]
    public void Constructor_WithEmptyList_ShouldCreateEmptyAllergens()
    {
        // Arrange
        var allergenList = new List<string>();

        // Act
        var allergens = new Allergens(allergenList);

        // Assert
        allergens.Values.Should().BeEmpty();
        allergens.HasAllergens.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNull_ShouldCreateEmptyAllergens()
    {
        // Act
        var allergens = new Allergens(null);

        // Assert
        allergens.Values.Should().BeEmpty();
        allergens.HasAllergens.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldRemoveDuplicates()
    {
        // Arrange
        var allergenList = new List<string> { "Gluten", "Dairy", "Gluten", "Nuts", "DAIRY" };

        // Act
        var allergens = new Allergens(allergenList);

        // Assert
        allergens.Values.Should().HaveCount(3);
        allergens.Values.Should().Contain("Gluten");
        allergens.Values.Should().Contain("Dairy");
        allergens.Values.Should().Contain("Nuts");
    }

    [Fact]
    public void Constructor_ShouldIgnoreEmptyStrings()
    {
        // Arrange
        var allergenList = new List<string> { "Gluten", "", "  ", null, "Dairy" };

        // Act
        var allergens = new Allergens(allergenList);

        // Assert
        allergens.Values.Should().HaveCount(2);
        allergens.Values.Should().Contain("Gluten");
        allergens.Values.Should().Contain("Dairy");
    }

    [Fact]
    public void HasAllergens_WithAllergens_ShouldReturnTrue()
    {
        // Arrange
        var allergens = new Allergens(new List<string> { "Gluten" });

        // Act & Assert
        allergens.HasAllergens.Should().BeTrue();
    }

    [Fact]
    public void Contains_ExistingAllergen_ShouldReturnTrue()
    {
        // Arrange
        var allergens = new Allergens(new List<string> { "Gluten", "Dairy" });

        // Act & Assert
        allergens.Contains("Gluten").Should().BeTrue();
        allergens.Contains("gluten").Should().BeTrue(); // Case insensitive
        allergens.Contains("Dairy").Should().BeTrue();
    }

    [Fact]
    public void Contains_NonExistingAllergen_ShouldReturnFalse()
    {
        // Arrange
        var allergens = new Allergens(new List<string> { "Gluten", "Dairy" });

        // Act & Assert
        allergens.Contains("Nuts").Should().BeFalse();
        allergens.Contains("Fish").Should().BeFalse();
    }

    [Fact]
    public void Equals_WithSameAllergens_ShouldReturnTrue()
    {
        // Arrange
        var allergens1 = new Allergens(new List<string> { "Gluten", "Dairy" });
        var allergens2 = new Allergens(new List<string> { "Dairy", "Gluten" }); // Order doesn't matter

        // Act & Assert
        allergens1.Should().Be(allergens2);
    }

    [Fact]
    public void Equals_WithDifferentAllergens_ShouldReturnFalse()
    {
        // Arrange
        var allergens1 = new Allergens(new List<string> { "Gluten", "Dairy" });
        var allergens2 = new Allergens(new List<string> { "Gluten", "Nuts" });

        // Act & Assert
        allergens1.Should().NotBe(allergens2);
    }

    [Fact]
    public void ToString_ShouldReturnCommaSeparatedList()
    {
        // Arrange
        var allergens = new Allergens(new List<string> { "Gluten", "Dairy", "Nuts" });

        // Act
        var result = allergens.ToString();

        // Assert
        result.Should().Contain("Gluten");
        result.Should().Contain("Dairy");
        result.Should().Contain("Nuts");
    }

    [Fact]
    public void ToString_WithNoAllergens_ShouldReturnNoneMessage()
    {
        // Arrange
        var allergens = new Allergens(new List<string>());

        // Act
        var result = allergens.ToString();

        // Assert
        result.Should().Contain("No allergens");
    }

    [Fact]
    public void None_ShouldReturnEmptyAllergens()
    {
        // Act
        var allergens = Allergens.None();

        // Assert
        allergens.Values.Should().BeEmpty();
        allergens.HasAllergens.Should().BeFalse();
    }
}
