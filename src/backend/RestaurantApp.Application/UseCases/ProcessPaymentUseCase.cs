using RestaurantApp.Application.DTOs;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.UseCases;

/// <summary>
/// Use case for processing a payment for an order
/// Orchestrates payment processing workflow
/// </summary>
public class ProcessPaymentUseCase
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;

    public ProcessPaymentUseCase(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IPaymentGateway paymentGateway)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
    }

    public async Task<Result<PaymentDto>> Execute(Guid orderId, string paymentMethod)
    {

        // 1. Validate order exists and is confirmed
        var order = await _orderRepository.GetById(OrderId.From(orderId));
        if (order == null)
        {
            return Result<PaymentDto>.Failure("Order not found");
        }

        // Allow payment for orders that have been confirmed and are in any stage after that
        // (Confirmed, Preparing, Ready, Delivered)
        if (order.Status == OrderStatus.Draft || order.Status == OrderStatus.Cancelled)
        {
            return Result<PaymentDto>.Failure($"Order must be confirmed before payment. Current status: {order.Status}");
        }

        // 2. Check if payment already exists for this order
        var existingPayment = await _paymentRepository.GetByOrderId(order.Id);
        if (existingPayment != null && existingPayment.IsSuccessful())
        {
            return Result<PaymentDto>.Failure("Payment has already been completed for this order");
        }

        // 3. Create payment entity
        var payment = Payment.Create(order.Id, order.Total);
        await _paymentRepository.Save(payment);

        // 4. Mark payment as processing
        payment.MarkAsProcessing();
        await _paymentRepository.Save(payment);

        // 5. Process payment through gateway
        var paymentRequest = new PaymentRequest(
            PaymentId: payment.Id,
            Amount: order.Total,
            PaymentMethod: paymentMethod,
            Metadata: new Dictionary<string, string>
            {
                { "orderId", orderId.ToString() },
                { "tableNumber", order.TableId.Value.ToString() }
            }
        );

        var paymentResult = await _paymentGateway.ProcessPayment(paymentRequest);

        // 6. Update payment based on gateway result
        if (paymentResult.Success)
        {
            payment.MarkAsCompleted(paymentResult.TransactionId!);
            await _paymentRepository.Save(payment);

            // TODO: Raise domain event for payment completed
            // This would trigger notifications, order status updates, etc.

            return Result<PaymentDto>.Success(MapToDto(payment));
        }
        else
        {
            var failureReason = $"{paymentResult.ErrorCode}: {paymentResult.ErrorMessage}";
            payment.MarkAsFailed(failureReason);
            await _paymentRepository.Save(payment);

            return Result<PaymentDto>.Failure($"Payment failed: {paymentResult.ErrorMessage}");
        }
    }

    private static PaymentDto MapToDto(Payment payment)
    {
        return new PaymentDto(
            Id: payment.Id.Value.ToString(),
            OrderId: payment.OrderId.Value.ToString(),
            Amount: payment.Amount.Amount,
            Currency: payment.Amount.Currency,
            Status: payment.Status.ToString(),
            TransactionId: payment.TransactionId,
            FailureReason: payment.FailureReason,
            CreatedAt: payment.CreatedAt,
            ProcessedAt: payment.ProcessedAt
        );
    }
}
