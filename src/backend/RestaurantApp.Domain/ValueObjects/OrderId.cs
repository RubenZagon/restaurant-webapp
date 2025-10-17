namespace RestaurantApp.Domain.ValueObjects;

public sealed record OrderId
{
    public Guid Value { get; }

    private OrderId(Guid value)
    {
        Value = value;
    }

    public static OrderId Create()
    {
        return new OrderId(Guid.NewGuid());
    }

    public static OrderId From(Guid value)
    {
        return new OrderId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator Guid(OrderId orderId) => orderId.Value;
}
