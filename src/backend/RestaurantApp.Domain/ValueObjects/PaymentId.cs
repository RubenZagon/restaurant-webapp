using RestaurantApp.Domain.Exceptions;

namespace RestaurantApp.Domain.ValueObjects;

public record PaymentId
{
    public Guid Value { get; }

    public PaymentId(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("Payment ID cannot be empty");

        Value = value;
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(PaymentId paymentId) => paymentId.Value;
}
