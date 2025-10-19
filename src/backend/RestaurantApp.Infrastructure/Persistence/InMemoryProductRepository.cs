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

        var bebidas = categoriesList.FirstOrDefault(c => c.Name == "Bebidas");
        var entrantes = categoriesList.FirstOrDefault(c => c.Name == "Entrantes");
        var carnes = categoriesList.FirstOrDefault(c => c.Name == "Carnes a la Brasa");
        var pescados = categoriesList.FirstOrDefault(c => c.Name == "Pescados");
        var guisos = categoriesList.FirstOrDefault(c => c.Name == "Guisos Canarios");
        var postres = categoriesList.FirstOrDefault(c => c.Name == "Postres");
        var cafes = categoriesList.FirstOrDefault(c => c.Name == "Cafés");

        // Bebidas
        if (bebidas != null)
        {
            AddProduct(Product.Create("Vino Tinto de la Casa", "Vino tinto del norte de Tenerife", new Price(1.50m, "EUR"), bebidas.Id, Allergens.None()));
            AddProduct(Product.Create("Vino Blanco de la Casa", "Vino blanco afrutado", new Price(1.50m, "EUR"), bebidas.Id, Allergens.None()));
            AddProduct(Product.Create("Vino Rosado", "Vino rosado fresco", new Price(1.50m, "EUR"), bebidas.Id, Allergens.None()));
            AddProduct(Product.Create("Agua", "Agua mineral", new Price(1.00m, "EUR"), bebidas.Id, Allergens.None()));
            AddProduct(Product.Create("Refresco", "Coca-Cola, Fanta, Sprite", new Price(1.50m, "EUR"), bebidas.Id, Allergens.None()));
            AddProduct(Product.Create("Cerveza Dorada", "Cerveza canaria", new Price(2.00m, "EUR"), bebidas.Id, Allergens.None()));
            AddProduct(Product.Create("Tropical", "Cerveza canaria", new Price(2.00m, "EUR"), bebidas.Id, Allergens.None()));
        }

        // Entrantes
        if (entrantes != null)
        {
            AddProduct(Product.Create("Papas Arrugadas con Mojo", "Papas con mojo picón y mojo verde", new Price(4.50m, "EUR"), entrantes.Id, Allergens.None()));
            AddProduct(Product.Create("Queso Asado", "Queso de cabra asado con mojo", new Price(6.00m, "EUR"), entrantes.Id, new Allergens(new[] { "lactosa" })));
            AddProduct(Product.Create("Pimientos de Padrón", "Pimientos fritos con sal gorda", new Price(5.00m, "EUR"), entrantes.Id, Allergens.None()));
            AddProduct(Product.Create("Chicharrones", "Chicharrones caseros", new Price(5.50m, "EUR"), entrantes.Id, Allergens.None()));
            AddProduct(Product.Create("Chorizo a la Brasa", "Chorizo canario a la brasa", new Price(6.50m, "EUR"), entrantes.Id, Allergens.None()));
            AddProduct(Product.Create("Champiñones al Ajillo", "Champiñones salteados con ajo", new Price(5.50m, "EUR"), entrantes.Id, Allergens.None()));
        }

        // Carnes a la Brasa
        if (carnes != null)
        {
            AddProduct(Product.Create("Chuletas de Cerdo", "Chuletas de cerdo a la brasa con papas y ensalada", new Price(9.50m, "EUR"), carnes.Id, Allergens.None()));
            AddProduct(Product.Create("Costillas", "Costillas de cerdo a la brasa", new Price(10.50m, "EUR"), carnes.Id, Allergens.None()));
            AddProduct(Product.Create("Pollo al Horno", "Medio pollo al horno con papas", new Price(8.50m, "EUR"), carnes.Id, Allergens.None()));
            AddProduct(Product.Create("Conejo al Salmorejo", "Conejo marinado en salmorejo canario", new Price(11.00m, "EUR"), carnes.Id, Allergens.None()));
            AddProduct(Product.Create("Carne de Cabra", "Carne de cabra guisada", new Price(10.50m, "EUR"), carnes.Id, Allergens.None()));
            AddProduct(Product.Create("Entrecot", "Entrecot de ternera a la brasa", new Price(14.00m, "EUR"), carnes.Id, Allergens.None()));
        }

        // Pescados
        if (pescados != null)
        {
            AddProduct(Product.Create("Cherne a la Plancha", "Cherne fresco con papas arrugadas", new Price(13.00m, "EUR"), pescados.Id, new Allergens(new[] { "pescado" })));
            AddProduct(Product.Create("Vieja Saneada", "Vieja a la plancha", new Price(12.00m, "EUR"), pescados.Id, new Allergens(new[] { "pescado" })));
            AddProduct(Product.Create("Sama a la Plancha", "Sama fresca con guarnición", new Price(13.50m, "EUR"), pescados.Id, new Allergens(new[] { "pescado" })));
            AddProduct(Product.Create("Pulpo a la Gallega", "Pulpo con papas y pimentón", new Price(14.00m, "EUR"), pescados.Id, new Allergens(new[] { "moluscos" })));
            AddProduct(Product.Create("Calamares Fritos", "Calamares rebozados", new Price(9.00m, "EUR"), pescados.Id, new Allergens(new[] { "moluscos", "gluten" })));
        }

        // Guisos Canarios
        if (guisos != null)
        {
            AddProduct(Product.Create("Ropa Vieja", "Guiso de garbanzos con carne", new Price(8.50m, "EUR"), guisos.Id, Allergens.None()));
            AddProduct(Product.Create("Potaje de Berros", "Potaje canario con berros y costilla", new Price(7.50m, "EUR"), guisos.Id, Allergens.None()));
            AddProduct(Product.Create("Puchero Canario", "Puchero con verduras y carnes", new Price(8.00m, "EUR"), guisos.Id, Allergens.None()));
            AddProduct(Product.Create("Rancho Canario", "Rancho con fideos y papas", new Price(7.00m, "EUR"), guisos.Id, new Allergens(new[] { "gluten" })));
        }

        // Postres
        if (postres != null)
        {
            AddProduct(Product.Create("Quesillo", "Flan canario casero", new Price(3.50m, "EUR"), postres.Id, new Allergens(new[] { "lactosa", "huevo" })));
            AddProduct(Product.Create("Bienmesabe", "Postre de almendras típico canario", new Price(4.00m, "EUR"), postres.Id, new Allergens(new[] { "frutos secos", "huevo" })));
            AddProduct(Product.Create("Frangollo", "Postre de gofio con leche", new Price(3.50m, "EUR"), postres.Id, new Allergens(new[] { "lactosa", "gluten" })));
            AddProduct(Product.Create("Príncipe Alberto", "Bizcocho con almendras y chocolate", new Price(4.00m, "EUR"), postres.Id, new Allergens(new[] { "gluten", "lactosa", "huevo", "frutos secos" })));
            AddProduct(Product.Create("Helado de la Casa", "Helado artesanal", new Price(3.00m, "EUR"), postres.Id, new Allergens(new[] { "lactosa" })));
        }

        // Cafés
        if (cafes != null)
        {
            AddProduct(Product.Create("Café Solo", "Café expreso", new Price(1.20m, "EUR"), cafes.Id, Allergens.None()));
            AddProduct(Product.Create("Cortado", "Café cortado", new Price(1.30m, "EUR"), cafes.Id, new Allergens(new[] { "lactosa" })));
            AddProduct(Product.Create("Café con Leche", "Café con leche", new Price(1.40m, "EUR"), cafes.Id, new Allergens(new[] { "lactosa" })));
            AddProduct(Product.Create("Barraquito", "Café canario con leche condensada y licor", new Price(2.00m, "EUR"), cafes.Id, new Allergens(new[] { "lactosa" })));
        }
    }

    private void AddProduct(Product product)
    {
        _products[product.Id.Value] = product;
    }
}
