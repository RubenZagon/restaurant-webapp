namespace RestaurantApp.Domain.ValueObjects;

public sealed record ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value)
    {
        Value = value;
    }

    public static ProductId Create()
    {
        return new ProductId(Guid.NewGuid());
    }

    public static ProductId From(Guid value)
    {
        return new ProductId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator Guid(ProductId productId) => productId.Value;
}
