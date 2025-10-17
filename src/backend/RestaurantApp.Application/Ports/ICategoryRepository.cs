using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.Ports;

public interface ICategoryRepository
{
    Task<Category?> GetById(CategoryId id);
    Task<IEnumerable<Category>> GetAll();
    Task<IEnumerable<Category>> GetActive();
    Task Save(Category category);
    Task Delete(CategoryId id);
}
