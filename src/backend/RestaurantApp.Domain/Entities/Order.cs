using RestaurantApp.Domain.Common;
using RestaurantApp.Domain.Events;
using RestaurantApp.Domain.Exceptions;
using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

public class Order : Entity
{
    private readonly List<OrderLine> _lines = new();

    public OrderId Id { get; private set; }
    public TableId TableId { get; private set; }
    public SessionId SessionId { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public OrderStatus Status { get; private set; }
    public Price Total { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }

    private Order(OrderId id, TableId tableId, SessionId sessionId)
    {
        Id = id;
        TableId = tableId;
        SessionId = sessionId;
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
        Total = new Price(0, "EUR");
    }

    public static Order Create(TableId tableId, SessionId sessionId)
    {
        return new Order(OrderId.Create(), tableId, sessionId);
    }

    public void AddProduct(ProductId productId, string productName, Price unitPrice, Quantity quantity)
    {
        EnsureCanBeModified();

        var existingLine = _lines.FirstOrDefault(l => l.ProductId.Value == productId.Value);

        if (existingLine != null)
        {
            var newQuantity = existingLine.Quantity.Add(quantity);
            existingLine.UpdateQuantity(newQuantity);
        }
        else
        {
            var newLine = OrderLine.Create(productId, productName, unitPrice, quantity);
            _lines.Add(newLine);
        }

        RecalculateTotal();
    }

    public void RemoveLine(OrderLineId lineId)
    {
        EnsureCanBeModified();

        var line = _lines.FirstOrDefault(l => l.Id.Value == lineId.Value);
        if (line == null)
        {
            throw new DomainException($"Order line with ID {lineId} not found.");
        }

        _lines.Remove(line);
        RecalculateTotal();
    }

    public void UpdateLineQuantity(OrderLineId lineId, Quantity newQuantity)
    {
        EnsureCanBeModified();

        var line = _lines.FirstOrDefault(l => l.Id.Value == lineId.Value);
        if (line == null)
        {
            throw new DomainException($"Order line with ID {lineId} not found.");
        }

        line.UpdateQuantity(newQuantity);
        RecalculateTotal();
    }

    public void Confirm()
    {
        if (_lines.Count == 0)
        {
            throw new DomainException("Cannot confirm an empty order.");
        }

        if (Status != OrderStatus.Draft)
        {
            throw new DomainException(
                $"Cannot confirm order in status {Status}. Only Draft orders can be confirmed.");
        }

        var oldStatus = Status;
        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTime.UtcNow;

        // Raise domain events
        RaiseDomainEvent(new OrderConfirmedEvent(Id, TableId.Value));
        RaiseDomainEvent(new OrderStatusChangedEvent(Id, TableId.Value, oldStatus, Status));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Delivered)
        {
            throw new DomainException("Cannot cancel an order that has been delivered.");
        }

        if (Status == OrderStatus.Cancelled)
        {
            throw new DomainException("Order is already cancelled.");
        }

        Status = OrderStatus.Cancelled;
    }

    public void MarkAsPreparing()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new DomainException("Only confirmed orders can be marked as preparing.");
        }

        var oldStatus = Status;
        Status = OrderStatus.Preparing;
        RaiseDomainEvent(new OrderStatusChangedEvent(Id, TableId.Value, oldStatus, Status));
    }

    public void MarkAsReady()
    {
        if (Status != OrderStatus.Preparing)
        {
            throw new DomainException("Only preparing orders can be marked as ready.");
        }

        var oldStatus = Status;
        Status = OrderStatus.Ready;
        RaiseDomainEvent(new OrderStatusChangedEvent(Id, TableId.Value, oldStatus, Status));
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Ready)
        {
            throw new DomainException("Only ready orders can be marked as delivered.");
        }

        var oldStatus = Status;
        Status = OrderStatus.Delivered;
        RaiseDomainEvent(new OrderStatusChangedEvent(Id, TableId.Value, oldStatus, Status));
    }

    private void EnsureCanBeModified()
    {
        if (Status != OrderStatus.Draft)
        {
            throw new DomainException(
                $"Cannot modify order in status {Status}. Only Draft orders can be modified.");
        }
    }

    private void RecalculateTotal()
    {
        if (_lines.Count == 0)
        {
            Total = new Price(0, "EUR");
            return;
        }

        var sum = _lines.Aggregate(
            new Price(0, "EUR"),
            (acc, line) => acc.Add(line.Subtotal));

        Total = sum;
    }
}
