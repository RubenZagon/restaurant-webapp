using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

public class Product
{
    private const int MaxNameLength = 200;

    public ProductId Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Price Price { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public Allergens Allergens { get; private set; }
    public bool IsAvailable { get; private set; }

    // Parameterless constructor for EF Core
    private Product()
    {
        Id = null!; // Will be set by EF Core
        Name = null!;
        Price = null!;
        CategoryId = null!;
        Allergens = null!;
    }

    private Product(
        ProductId id,
        string name,
        string? description,
        Price price,
        CategoryId categoryId,
        Allergens allergens)
    {
        Id = id;
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        Allergens = allergens;
        IsAvailable = true;
    }

    public static Product Create(
        string name,
        string? description,
        Price price,
        CategoryId categoryId,
        Allergens allergens)
    {
        ValidateName(name);

        return new Product(
            ProductId.Create(),
            name,
            description,
            price,
            categoryId,
            allergens ?? Allergens.None());
    }

    public void UpdateName(string newName)
    {
        ValidateName(newName);
        Name = newName;
    }

    public void UpdateDescription(string? newDescription)
    {
        Description = newDescription;
    }

    public void UpdatePrice(Price newPrice)
    {
        Price = newPrice ?? throw new DomainException("Price cannot be null");
    }

    public void UpdateCategory(CategoryId newCategoryId)
    {
        CategoryId = newCategoryId ?? throw new DomainException("CategoryId cannot be null");
    }

    public void MakeAvailable()
    {
        IsAvailable = true;
    }

    public void MakeUnavailable()
    {
        IsAvailable = false;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }

        if (name.Length > MaxNameLength)
        {
            throw new DomainException(
                $"Product name cannot exceed {MaxNameLength} characters. Current length: {name.Length}");
        }
    }
}
