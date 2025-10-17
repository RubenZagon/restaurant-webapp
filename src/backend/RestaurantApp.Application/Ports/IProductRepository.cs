using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.Ports;

public interface IProductRepository
{
    Task<Product?> GetById(ProductId id);
    Task<IEnumerable<Product>> GetAll();
    Task<IEnumerable<Product>> GetByCategory(CategoryId categoryId);
    Task<IEnumerable<Product>> GetAvailable();
    Task Save(Product product);
    Task Delete(ProductId id);
}
