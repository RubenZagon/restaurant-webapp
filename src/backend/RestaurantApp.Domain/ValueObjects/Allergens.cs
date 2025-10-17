namespace RestaurantApp.Domain.ValueObjects;

public sealed record Allergens
{
    public IReadOnlySet<string> Values { get; }
    public bool HasAllergens => Values.Any();

    public Allergens(IEnumerable<string>? allergens)
    {
        if (allergens == null)
        {
            Values = new HashSet<string>();
            return;
        }

        var normalized = allergens
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(a => NormalizeAllergen(a))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Values = normalized;
    }

    private static string NormalizeAllergen(string allergen)
    {
        var trimmed = allergen.Trim();
        return char.ToUpper(trimmed[0]) + trimmed[1..].ToLower();
    }

    public bool Contains(string allergen)
    {
        if (string.IsNullOrWhiteSpace(allergen))
            return false;

        return Values.Contains(allergen, StringComparer.OrdinalIgnoreCase);
    }

    public static Allergens None()
    {
        return new Allergens(null);
    }

    public override string ToString()
    {
        if (!HasAllergens)
            return "No allergens";

        return string.Join(", ", Values.OrderBy(a => a));
    }

    public bool Equals(Allergens? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Values.SetEquals(other.Values);
    }

    public override int GetHashCode()
    {
        return Values.Aggregate(0, (acc, value) =>
            acc ^ value.GetHashCode(StringComparison.OrdinalIgnoreCase));
    }
}
