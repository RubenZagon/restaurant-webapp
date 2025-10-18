using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Application.Ports;

/// <summary>
/// Repository port for Payment aggregate persistence
/// </summary>
public interface IPaymentRepository
{
    /// <summary>
    /// Gets a payment by its ID
    /// </summary>
    Task<Payment?> GetById(PaymentId id);

    /// <summary>
    /// Gets a payment by order ID
    /// </summary>
    Task<Payment?> GetByOrderId(OrderId orderId);

    /// <summary>
    /// Saves a payment (insert or update)
    /// </summary>
    Task Save(Payment payment);

    /// <summary>
    /// Gets all payments for a specific table
    /// </summary>
    Task<IEnumerable<Payment>> GetByTableId(TableId tableId);
}
