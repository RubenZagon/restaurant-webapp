using RestaurantApp.Domain.ValueObjects;

namespace RestaurantApp.Domain.Entities;

public class OrderLine
{
    public OrderLineId Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }
    public Price UnitPrice { get; private set; }
    public Quantity Quantity { get; private set; }
    public Price Subtotal { get; private set; }

    // Parameterless constructor for EF Core
    private OrderLine()
    {
        Id = null!; // Will be set by EF Core
        ProductId = null!;
        ProductName = null!;
        UnitPrice = null!;
        Quantity = null!;
        Subtotal = null!;
    }

    private OrderLine(
        OrderLineId id,
        ProductId productId,
        string productName,
        Price unitPrice,
        Quantity quantity)
    {
        Id = id;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Subtotal = CalculateSubtotal();
    }

    public static OrderLine Create(
        ProductId productId,
        string productName,
        Price unitPrice,
        Quantity quantity)
    {
        return new OrderLine(
            OrderLineId.Create(),
            productId,
            productName,
            unitPrice,
            quantity);
    }

    public void UpdateQuantity(Quantity newQuantity)
    {
        Quantity = newQuantity;
        Subtotal = CalculateSubtotal();
    }

    private Price CalculateSubtotal()
    {
        return UnitPrice.Multiply(Quantity.Value);
    }
}
