using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.UseCases;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TablesController : ControllerBase
{
    private readonly ILogger<TablesController> _logger;
    private readonly StartTableSessionUseCase _startSessionUseCase;

    public TablesController(
        ILogger<TablesController> logger,
        StartTableSessionUseCase startSessionUseCase)
    {
        _logger = logger;
        _startSessionUseCase = startSessionUseCase;
    }

    [HttpPost("{tableNumber}/start-session")]
    public async Task<IActionResult> StartSession(int tableNumber)
    {
        _logger.LogInformation("Starting session for table {TableNumber}", tableNumber);

        var result = await _startSessionUseCase.Execute(tableNumber);

        if (!result.IsSuccess)
        {
            _logger.LogWarning(
                "Error starting session for table {TableNumber}: {Error}",
                tableNumber,
                result.Error);
            return BadRequest(new { error = result.Error });
        }

        _logger.LogInformation(
            "Session started for table {TableNumber} with ID {SessionId}",
            tableNumber,
            result.Value!.SessionId);

        return Ok(result.Value);
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
    }
}
