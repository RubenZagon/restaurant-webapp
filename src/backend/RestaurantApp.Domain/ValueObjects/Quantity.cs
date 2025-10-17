using RestaurantApp.Domain.Exceptions;

namespace RestaurantApp.Domain.ValueObjects;

public sealed record Quantity
{
    private const int MinValue = 1;
    private const int MaxValue = 100;

    public int Value { get; }

    public Quantity(int value)
    {
        if (value < MinValue)
        {
            throw new DomainException(
                $"Quantity must be at least {MinValue}. Received: {value}");
        }

        if (value > MaxValue)
        {
            throw new DomainException(
                $"Quantity cannot exceed {MaxValue}. Received: {value}");
        }

        Value = value;
    }

    public Quantity Add(Quantity other)
    {
        var newValue = Value + other.Value;

        if (newValue > MaxValue)
        {
            throw new DomainException(
                $"Total quantity cannot exceed {MaxValue}. Attempted: {newValue}");
        }

        return new Quantity(newValue);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator int(Quantity quantity) => quantity.Value;
}
