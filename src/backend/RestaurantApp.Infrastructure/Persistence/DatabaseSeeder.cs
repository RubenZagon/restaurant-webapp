using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence;

/// <summary>
/// Seeds initial data into the database
/// </summary>
public class DatabaseSeeder
{
    private readonly RestaurantDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(RestaurantDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Check if data already exists
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Database already seeded, skipping seed");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            await SeedTables();
            await SeedCategoriesAndProducts();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding database");
            throw;
        }
    }

    private async Task SeedTables()
    {
        _logger.LogInformation("Seeding tables...");

        // Create 7 tables (numbered 1-7)
        for (int i = 1; i <= 7; i++)
        {
            var tableId = new TableId(i);
            var table = new Table(tableId);
            await _context.Tables.AddAsync(table);
        }

        _logger.LogInformation("Created 7 tables");
    }

    private async Task SeedCategoriesAndProducts()
    {
        // Bebidas
        var bebidasCategory = Category.Create("Bebidas", "Vinos de la casa y bebidas");
        await _context.Categories.AddAsync(bebidasCategory);

        var bebidas = new[]
        {
            Product.Create("Vino Tinto de la Casa", "Vino tinto del norte de Tenerife", new Price(1.50m, "EUR"), bebidasCategory.Id, Allergens.None()),
            Product.Create("Vino Blanco de la Casa", "Vino blanco afrutado", new Price(1.50m, "EUR"), bebidasCategory.Id, Allergens.None()),
            Product.Create("Vino Rosado", "Vino rosado fresco", new Price(1.50m, "EUR"), bebidasCategory.Id, Allergens.None()),
            Product.Create("Agua", "Agua mineral", new Price(1.00m, "EUR"), bebidasCategory.Id, Allergens.None()),
            Product.Create("Refresco", "Coca-Cola, Fanta, Sprite", new Price(1.50m, "EUR"), bebidasCategory.Id, Allergens.None()),
            Product.Create("Cerveza Dorada", "Cerveza canaria", new Price(2.00m, "EUR"), bebidasCategory.Id, Allergens.None()),
            Product.Create("Tropical", "Cerveza canaria", new Price(2.00m, "EUR"), bebidasCategory.Id, Allergens.None()),
        };
        await _context.Products.AddRangeAsync(bebidas);

        // Entrantes
        var entrantesCategory = Category.Create("Entrantes", "Para abrir boca");
        await _context.Categories.AddAsync(entrantesCategory);

        var entrantes = new[]
        {
            Product.Create("Papas Arrugadas con Mojo", "Papas con mojo picón y mojo verde", new Price(4.50m, "EUR"), entrantesCategory.Id, Allergens.None()),
            Product.Create("Queso Asado", "Queso de cabra asado con mojo", new Price(6.00m, "EUR"), entrantesCategory.Id, new Allergens(new[] { "lactosa" })),
            Product.Create("Pimientos de Padrón", "Pimientos fritos con sal gorda", new Price(5.00m, "EUR"), entrantesCategory.Id, Allergens.None()),
            Product.Create("Chicharrones", "Chicharrones caseros", new Price(5.50m, "EUR"), entrantesCategory.Id, Allergens.None()),
            Product.Create("Chorizo a la Brasa", "Chorizo canario a la brasa", new Price(6.50m, "EUR"), entrantesCategory.Id, Allergens.None()),
            Product.Create("Champiñones al Ajillo", "Champiñones salteados con ajo", new Price(5.50m, "EUR"), entrantesCategory.Id, Allergens.None()),
        };
        await _context.Products.AddRangeAsync(entrantes);

        // Carnes a la Brasa
        var carnesCategory = Category.Create("Carnes a la Brasa", "Nuestras especialidades a la parrilla");
        await _context.Categories.AddAsync(carnesCategory);

        var carnes = new[]
        {
            Product.Create("Chuletas de Cerdo", "Chuletas de cerdo a la brasa con papas y ensalada", new Price(9.50m, "EUR"), carnesCategory.Id, Allergens.None()),
            Product.Create("Costillas", "Costillas de cerdo a la brasa", new Price(10.50m, "EUR"), carnesCategory.Id, Allergens.None()),
            Product.Create("Pollo al Horno", "Medio pollo al horno con papas", new Price(8.50m, "EUR"), carnesCategory.Id, Allergens.None()),
            Product.Create("Conejo al Salmorejo", "Conejo marinado en salmorejo canario", new Price(11.00m, "EUR"), carnesCategory.Id, Allergens.None()),
            Product.Create("Carne de Cabra", "Carne de cabra guisada", new Price(10.50m, "EUR"), carnesCategory.Id, Allergens.None()),
            Product.Create("Entrecot", "Entrecot de ternera a la brasa", new Price(14.00m, "EUR"), carnesCategory.Id, Allergens.None()),
        };
        await _context.Products.AddRangeAsync(carnes);

        // Pescados
        var pescadosCategory = Category.Create("Pescados", "Pescado fresco del día");
        await _context.Categories.AddAsync(pescadosCategory);

        var pescados = new[]
        {
            Product.Create("Cherne a la Plancha", "Cherne fresco con papas arrugadas", new Price(13.00m, "EUR"), pescadosCategory.Id, new Allergens(new[] { "pescado" })),
            Product.Create("Vieja Saneada", "Vieja a la plancha", new Price(12.00m, "EUR"), pescadosCategory.Id, new Allergens(new[] { "pescado" })),
            Product.Create("Sama a la Plancha", "Sama fresca con guarnición", new Price(13.50m, "EUR"), pescadosCategory.Id, new Allergens(new[] { "pescado" })),
            Product.Create("Pulpo a la Gallega", "Pulpo con papas y pimentón", new Price(14.00m, "EUR"), pescadosCategory.Id, new Allergens(new[] { "moluscos" })),
            Product.Create("Calamares Fritos", "Calamares rebozados", new Price(9.00m, "EUR"), pescadosCategory.Id, new Allergens(new[] { "moluscos", "gluten" })),
        };
        await _context.Products.AddRangeAsync(pescados);

        // Guisos
        var guisosCategory = Category.Create("Guisos Canarios", "Potajes y guisos tradicionales");
        await _context.Categories.AddAsync(guisosCategory);

        var guisos = new[]
        {
            Product.Create("Ropa Vieja", "Guiso de garbanzos con carne", new Price(8.50m, "EUR"), guisosCategory.Id, Allergens.None()),
            Product.Create("Potaje de Berros", "Potaje canario con berros y costilla", new Price(7.50m, "EUR"), guisosCategory.Id, Allergens.None()),
            Product.Create("Puchero Canario", "Puchero con verduras y carnes", new Price(8.00m, "EUR"), guisosCategory.Id, Allergens.None()),
            Product.Create("Rancho Canario", "Rancho con fideos y papas", new Price(7.00m, "EUR"), guisosCategory.Id, new Allergens(new[] { "gluten" })),
        };
        await _context.Products.AddRangeAsync(guisos);

        // Postres
        var postresCategory = Category.Create("Postres", "Postres caseros");
        await _context.Categories.AddAsync(postresCategory);

        var postres = new[]
        {
            Product.Create("Quesillo", "Flan canario casero", new Price(3.50m, "EUR"), postresCategory.Id, new Allergens(new[] { "lactosa", "huevo" })),
            Product.Create("Bienmesabe", "Postre de almendras típico canario", new Price(4.00m, "EUR"), postresCategory.Id, new Allergens(new[] { "frutos secos", "huevo" })),
            Product.Create("Frangollo", "Postre de gofio con leche", new Price(3.50m, "EUR"), postresCategory.Id, new Allergens(new[] { "lactosa", "gluten" })),
            Product.Create("Príncipe Alberto", "Bizcocho con almendras y chocolate", new Price(4.00m, "EUR"), postresCategory.Id, new Allergens(new[] { "gluten", "lactosa", "huevo", "frutos secos" })),
            Product.Create("Helado de la Casa", "Helado artesanal", new Price(3.00m, "EUR"), postresCategory.Id, new Allergens(new[] { "lactosa" })),
        };
        await _context.Products.AddRangeAsync(postres);

        // Cafés
        var cafesCategory = Category.Create("Cafés", "Café y bebidas calientes");
        await _context.Categories.AddAsync(cafesCategory);

        var cafes = new[]
        {
            Product.Create("Café Solo", "Café expreso", new Price(1.20m, "EUR"), cafesCategory.Id, Allergens.None()),
            Product.Create("Cortado", "Café cortado", new Price(1.30m, "EUR"), cafesCategory.Id, new Allergens(new[] { "lactosa" })),
            Product.Create("Café con Leche", "Café con leche", new Price(1.40m, "EUR"), cafesCategory.Id, new Allergens(new[] { "lactosa" })),
            Product.Create("Barraquito", "Café canario con leche condensada y licor", new Price(2.00m, "EUR"), cafesCategory.Id, new Allergens(new[] { "lactosa" })),
        };
        await _context.Products.AddRangeAsync(cafes);
    }
}
