using RestaurantApp.Domain.Exceptions;

namespace RestaurantApp.Domain.ValueObjects;

public sealed record TableId
{
    public int Value { get; }

    public TableId(int value)
    {
        if (value <= 0)
        {
            throw new DomainException(
                $"Table number must be positive. Received value: {value}");
        }

        Value = value;
    }

    public override string ToString()
    {
        return $"Table {Value}";
    }

    public static implicit operator int(TableId tableId) => tableId.Value;
}
