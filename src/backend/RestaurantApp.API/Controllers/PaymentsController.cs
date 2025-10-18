using Microsoft.AspNetCore.Mvc;
using RestaurantApp.Application.UseCases;

namespace RestaurantApp.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ProcessPaymentUseCase _processPaymentUseCase;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(
        ProcessPaymentUseCase processPaymentUseCase,
        ILogger<PaymentsController> logger)
    {
        _processPaymentUseCase = processPaymentUseCase;
        _logger = logger;
    }

    /// <summary>
    /// Process a payment for an order
    /// </summary>
    /// <param name="orderId">The order ID to pay for</param>
    /// <param name="request">Payment processing request</param>
    /// <returns>Payment result</returns>
    [HttpPost("orders/{orderId}")]
    public async Task<IActionResult> ProcessPayment(
        Guid orderId,
        [FromBody] ProcessPaymentRequest request)
    {
        _logger.LogInformation(
            "POST /api/payments/orders/{OrderId} - PaymentMethod: {PaymentMethod}",
            orderId, request.PaymentMethod);

        var result = await _processPaymentUseCase.Execute(orderId, request.PaymentMethod);

        if (result.IsSuccess)
        {
            return Ok(new
            {
                success = true,
                data = result.Value
            });
        }

        return BadRequest(new
        {
            success = false,
            error = result.Error
        });
    }
}

/// <summary>
/// Request model for processing a payment
/// </summary>
public record ProcessPaymentRequest(
    string PaymentMethod  // e.g., "credit_card", "debit_card", "cash"
);
