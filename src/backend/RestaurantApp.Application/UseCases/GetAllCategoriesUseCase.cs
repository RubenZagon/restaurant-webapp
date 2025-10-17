using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;

namespace RestaurantApp.Application.UseCases;

public class GetAllCategoriesUseCase
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesUseCase(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<IEnumerable<CategoryDto>>> Execute()
    {
        var categories = await _categoryRepository.GetActive();

        var categoryDtos = categories
            .Select(c => new CategoryDto(
                c.Id.Value,
                c.Name,
                c.Description,
                c.IsActive))
            .ToList();

        return Result<IEnumerable<CategoryDto>>.Success(categoryDtos);
    }
}
