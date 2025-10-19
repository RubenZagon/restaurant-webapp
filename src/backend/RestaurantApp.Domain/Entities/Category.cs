using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

public class Category
{
    private const int MaxNameLength = 100;

    public CategoryId Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    // Parameterless constructor for EF Core
    private Category()
    {
        Id = null!; // Will be set by EF Core
        Name = null!;
    }

    private Category(CategoryId id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
        IsActive = true;
    }

    public static Category Create(string name, string? description = null)
    {
        ValidateName(name);

        return new Category(CategoryId.Create(), name, description);
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

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name is required.");
        }

        if (name.Length > MaxNameLength)
        {
            throw new DomainException(
                $"Category name cannot exceed {MaxNameLength} characters. Current length: {name.Length}");
        }
    }
}
