namespace RestaurantApp.Domain.ValueObjects;

public sealed record OrderLineId
{
    public Guid Value { get; }

    private OrderLineId(Guid value)
    {
        Value = value;
    }

    public static OrderLineId Create()
    {
        return new OrderLineId(Guid.NewGuid());
    }

    public static OrderLineId From(Guid value)
    {
        return new OrderLineId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator Guid(OrderLineId orderLineId) => orderLineId.Value;
}
