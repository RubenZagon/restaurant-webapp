using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.UseCases;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly ILogger<OrdersController> _logger;
    private readonly GetOrCreateOrderForTableUseCase _getOrCreateOrderUseCase;
    private readonly AddProductToOrderUseCase _addProductUseCase;

    public OrdersController(
        ILogger<OrdersController> logger,
        GetOrCreateOrderForTableUseCase getOrCreateOrderUseCase,
        AddProductToOrderUseCase addProductUseCase)
    {
        _logger = logger;
        _getOrCreateOrderUseCase = getOrCreateOrderUseCase;
        _addProductUseCase = addProductUseCase;
    }

    [HttpGet("table/{tableNumber}")]
    public async Task<IActionResult> GetOrCreateOrderForTable(int tableNumber)
    {
        _logger.LogInformation("Getting or creating order for table {TableNumber}", tableNumber);

        var result = await _getOrCreateOrderUseCase.Execute(tableNumber);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Error getting order for table {TableNumber}: {Error}", tableNumber, result.Error);
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    [HttpPost("{orderId}/products")]
    public async Task<IActionResult> AddProduct(Guid orderId, [FromBody] AddProductToOrderRequest request)
    {
        _logger.LogInformation(
            "Adding product {ProductId} to order {OrderId}",
            request.ProductId,
            orderId);

        var result = await _addProductUseCase.Execute(orderId, request.ProductId, request.Quantity);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Error adding product to order: {Error}", result.Error);
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }
}
