namespace RestaurantApp.Domain.ValueObjects;

public sealed record CategoryId
{
    public Guid Value { get; }

    private CategoryId(Guid value)
    {
        Value = value;
    }

    public static CategoryId Create()
    {
        return new CategoryId(Guid.NewGuid());
    }

    public static CategoryId From(Guid value)
    {
        return new CategoryId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator Guid(CategoryId categoryId) => categoryId.Value;
}
