using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

public class GetProductsByCategoryUseCase
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetProductsByCategoryUseCase(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IEnumerable<ProductDto>>> Execute(Guid categoryId)
    {
        var catId = CategoryId.From(categoryId);
        var category = await _categoryRepository.GetById(catId);

        if (category == null)
        {
            return Result<IEnumerable<ProductDto>>.Failure(
                $"Category with ID {categoryId} does not exist.");
        }

        if (!category.IsActive)
        {
            return Result<IEnumerable<ProductDto>>.Failure(
                $"Category '{category.Name}' is not active.");
        }

        var products = await _productRepository.GetByCategory(catId);

        var productDtos = products
            .Where(p => p.IsAvailable)
            .Select(p => new ProductDto(
                p.Id.Value,
                p.Name,
                p.Description,
                p.Price.Amount,
                p.Price.Currency,
                p.CategoryId.Value,
                p.Allergens.Values,
                p.IsAvailable))
            .ToList();

        return Result<IEnumerable<ProductDto>>.Success(productDtos);
    }
}
