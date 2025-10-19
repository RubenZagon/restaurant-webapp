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
        var bebidas = Category.Create("Bebidas", "Vinos de la casa y bebidas");
        var entrantes = Category.Create("Entrantes", "Para abrir boca");
        var carnes = Category.Create("Carnes a la Brasa", "Nuestras especialidades a la parrilla");
        var pescados = Category.Create("Pescados", "Pescado fresco del día");
        var guisos = Category.Create("Guisos Canarios", "Potajes y guisos tradicionales");
        var postres = Category.Create("Postres", "Postres caseros");
        var cafes = Category.Create("Cafés", "Café y bebidas calientes");

        _categories[bebidas.Id.Value] = bebidas;
        _categories[entrantes.Id.Value] = entrantes;
        _categories[carnes.Id.Value] = carnes;
        _categories[pescados.Id.Value] = pescados;
        _categories[guisos.Id.Value] = guisos;
        _categories[postres.Id.Value] = postres;
        _categories[cafes.Id.Value] = cafes;
    }
}
