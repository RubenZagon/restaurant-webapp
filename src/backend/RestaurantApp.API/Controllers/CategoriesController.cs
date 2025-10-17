using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.UseCases;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ILogger<CategoriesController> _logger;
    private readonly GetAllCategoriesUseCase _getAllCategoriesUseCase;

    public CategoriesController(
        ILogger<CategoriesController> logger,
        GetAllCategoriesUseCase getAllCategoriesUseCase)
    {
        _logger = logger;
        _getAllCategoriesUseCase = getAllCategoriesUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Getting all active categories");

        var result = await _getAllCategoriesUseCase.Execute();

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Error getting categories: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation("Retrieved {Count} categories", result.Value?.Count() ?? 0);
        return Ok(result.Value);
    }
}
