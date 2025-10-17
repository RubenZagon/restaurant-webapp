using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace RestaurantApp.Infrastructure.Persistence;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly ConcurrentDictionary<Guid, Category> _categories = new();

    public InMemoryCategoryRepository()
    {
        SeedData();
    }

    public Task<Category?> GetById(CategoryId id)
    {
        _categories.TryGetValue(id.Value, out var category);
        return Task.FromResult(category);
    }

    public Task<IEnumerable<Category>> GetAll()
    {
        return Task.FromResult(_categories.Values.AsEnumerable());
    }

    public Task<IEnumerable<Category>> GetActive()
    {
        return Task.FromResult(_categories.Values.Where(c => c.IsActive).AsEnumerable());
    }

    public Task Save(Category category)
    {
        _categories[category.Id.Value] = category;
        return Task.CompletedTask;
    }

    public Task Delete(CategoryId id)
    {
        _categories.TryRemove(id.Value, out _);
        return Task.CompletedTask;
    }

    private void SeedData()
    {
        var starters = Category.Create("Starters", "Delicious appetizers to start your meal");
        var mains = Category.Create("Main Courses", "Our signature main dishes");
        var desserts = Category.Create("Desserts", "Sweet treats to finish your meal");
        var drinks = Category.Create("Drinks", "Beverages and cocktails");

        _categories[starters.Id.Value] = starters;
        _categories[mains.Id.Value] = mains;
        _categories[desserts.Id.Value] = desserts;
        _categories[drinks.Id.Value] = drinks;
    }
}
