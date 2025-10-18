using System.Collections.Concurrent;
using RestaurantApp.Application.Ports;
using RestaurantApp.Domain.Entities;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of IPaymentRepository for development and testing
/// </summary>
public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly ConcurrentDictionary<Guid, Payment> _payments = new();

    public Task<Payment?> GetById(PaymentId id)
    {
        _payments.TryGetValue(id.Value, out var payment);
        return Task.FromResult(payment);
    }

    public Task<Payment?> GetByOrderId(OrderId orderId)
    {
        var payment = _payments.Values.FirstOrDefault(p => p.OrderId.Value == orderId.Value);
        return Task.FromResult(payment);
    }

    public Task Save(Payment payment)
    {
        _payments[payment.Id.Value] = payment;
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Payment>> GetByTableId(TableId tableId)
    {
        // Note: This requires accessing the Order to get TableId
        // For now, returning empty as Payment doesn't directly have TableId
        // In a real implementation, this would join with Orders
        var payments = _payments.Values.AsEnumerable();
        return Task.FromResult(payments);
    }
}
