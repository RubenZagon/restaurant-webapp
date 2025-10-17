using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.UseCases;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;
    private readonly GetProductsByCategoryUseCase _getProductsByCategoryUseCase;

    public ProductsController(
        ILogger<ProductsController> logger,
        GetProductsByCategoryUseCase getProductsByCategoryUseCase)
    {
        _logger = logger;
        _getProductsByCategoryUseCase = getProductsByCategoryUseCase;
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(Guid categoryId)
    {
        _logger.LogInformation("Getting products for category {CategoryId}", categoryId);

        var result = await _getProductsByCategoryUseCase.Execute(categoryId);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Error getting products for category {CategoryId}: {Error}",
                categoryId,
                result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation(
            "Retrieved {Count} products for category {CategoryId}",
            result.Value?.Count() ?? 0,
            categoryId);

        return Ok(result.Value);
    }
}
