using RestaurantApp.Domain.Exceptions;

namespace RestaurantApp.Domain.ValueObjects;

public sealed record Price
{
    private static readonly HashSet<string> ValidCurrencies = new()
    {
        "EUR", "USD", "GBP", "JPY", "CHF", "CAD", "AUD", "CNY"
    };

    public decimal Amount { get; }
    public string Currency { get; }

    public Price(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new DomainException(
                $"Price amount cannot be negative. Received: {amount}");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required.");
        }

        var normalizedCurrency = currency.Trim().ToUpper();

        if (normalizedCurrency.Length != 3)
        {
            throw new DomainException(
                $"Currency must be a valid 3-letter ISO code. Received: {currency}");
        }

        if (!ValidCurrencies.Contains(normalizedCurrency))
        {
            throw new DomainException(
                $"Currency '{normalizedCurrency}' is not supported. Supported currencies: {string.Join(", ", ValidCurrencies)}");
        }

        Amount = amount;
        Currency = normalizedCurrency;
    }

    public Price Add(Price other)
    {
        if (Currency != other.Currency)
        {
            throw new DomainException(
                $"Cannot add prices with different currencies: {Currency} and {other.Currency}. Prices must have the same currency.");
        }

        return new Price(Amount + other.Amount, Currency);
    }

    public Price Multiply(int quantity)
    {
        if (quantity <= 0)
        {
            throw new DomainException(
                $"Quantity must be positive. Received: {quantity}");
        }

        return new Price(Amount * quantity, Currency);
    }

    public override string ToString()
    {
        return $"{Amount:F2} {Currency}";
    }
}
