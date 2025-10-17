using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace RestaurantApp.Infrastructure.Persistence;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<Guid, Product> _products = new();
    private readonly ICategoryRepository _categoryRepository;

    public InMemoryProductRepository(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
        Task.Run(SeedData).Wait();
    }

    public Task<Product?> GetById(ProductId id)
    {
        _products.TryGetValue(id.Value, out var product);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetAll()
    {
        return Task.FromResult(_products.Values.AsEnumerable());
    }

    public Task<IEnumerable<Product>> GetByCategory(CategoryId categoryId)
    {
        var products = _products.Values.Where(p => p.CategoryId.Value == categoryId.Value);
        return Task.FromResult(products.AsEnumerable());
    }

    public Task<IEnumerable<Product>> GetAvailable()
    {
        var products = _products.Values.Where(p => p.IsAvailable);
        return Task.FromResult(products.AsEnumerable());
    }

    public Task Save(Product product)
    {
        _products[product.Id.Value] = product;
        return Task.CompletedTask;
    }

    public Task Delete(ProductId id)
    {
        _products.TryRemove(id.Value, out _);
        return Task.CompletedTask;
    }

    private async Task SeedData()
    {
        var categories = await _categoryRepository.GetAll();
        var categoriesList = categories.ToList();

        if (!categoriesList.Any()) return;

        var starters = categoriesList.FirstOrDefault(c => c.Name == "Starters");
        var mains = categoriesList.FirstOrDefault(c => c.Name == "Main Courses");
        var desserts = categoriesList.FirstOrDefault(c => c.Name == "Desserts");
        var drinks = categoriesList.FirstOrDefault(c => c.Name == "Drinks");

        // Starters
        if (starters != null)
        {
            AddProduct(Product.Create(
                "Bruschetta",
                "Toasted bread with tomatoes, garlic, and basil",
                new Price(6.50m, "EUR"),
                starters.Id,
                new Allergens(new[] { "Gluten" })
            ));

            AddProduct(Product.Create(
                "Calamari",
                "Crispy fried squid with lemon",
                new Price(8.90m, "EUR"),
                starters.Id,
                new Allergens(new[] { "Shellfish", "Gluten" })
            ));

            AddProduct(Product.Create(
                "Caprese Salad",
                "Fresh mozzarella, tomatoes, and basil",
                new Price(7.50m, "EUR"),
                starters.Id,
                new Allergens(new[] { "Dairy" })
            ));
        }

        // Main Courses
        if (mains != null)
        {
            AddProduct(Product.Create(
                "Paella",
                "Traditional Spanish rice with seafood",
                new Price(18.90m, "EUR"),
                mains.Id,
                new Allergens(new[] { "Shellfish", "Fish" })
            ));

            AddProduct(Product.Create(
                "Ribeye Steak",
                "Prime beef with garlic butter",
                new Price(24.90m, "EUR"),
                mains.Id,
                Allergens.None()
            ));

            AddProduct(Product.Create(
                "Vegetarian Lasagna",
                "Layers of pasta with vegetables and cheese",
                new Price(14.50m, "EUR"),
                mains.Id,
                new Allergens(new[] { "Gluten", "Dairy", "Eggs" })
            ));

            AddProduct(Product.Create(
                "Grilled Salmon",
                "Fresh Atlantic salmon with herbs",
                new Price(19.90m, "EUR"),
                mains.Id,
                new Allergens(new[] { "Fish" })
            ));
        }

        // Desserts
        if (desserts != null)
        {
            AddProduct(Product.Create(
                "Tiramisu",
                "Classic Italian coffee-flavored dessert",
                new Price(6.50m, "EUR"),
                desserts.Id,
                new Allergens(new[] { "Gluten", "Dairy", "Eggs" })
            ));

            AddProduct(Product.Create(
                "Chocolate Lava Cake",
                "Warm chocolate cake with molten center",
                new Price(7.50m, "EUR"),
                desserts.Id,
                new Allergens(new[] { "Gluten", "Dairy", "Eggs" })
            ));

            AddProduct(Product.Create(
                "Fruit Sorbet",
                "Refreshing fruit ice cream",
                new Price(5.50m, "EUR"),
                desserts.Id,
                Allergens.None()
            ));
        }

        // Drinks
        if (drinks != null)
        {
            AddProduct(Product.Create(
                "Coca-Cola",
                "Classic soft drink",
                new Price(2.50m, "EUR"),
                drinks.Id,
                Allergens.None()
            ));

            AddProduct(Product.Create(
                "House Wine (Red)",
                "Spanish red wine, glass",
                new Price(4.50m, "EUR"),
                drinks.Id,
                new Allergens(new[] { "Sulfites" })
            ));

            AddProduct(Product.Create(
                "Beer",
                "Local draft beer",
                new Price(3.50m, "EUR"),
                drinks.Id,
                new Allergens(new[] { "Gluten" })
            ));

            AddProduct(Product.Create(
                "Sparkling Water",
                "San Pellegrino 500ml",
                new Price(2.00m, "EUR"),
                drinks.Id,
                Allergens.None()
            ));
        }
    }

    private void AddProduct(Product product)
    {
        _products[product.Id.Value] = product;
    }
}
