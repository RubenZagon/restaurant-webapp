namespace RestaurantApp.Domain.ValueObjects;

public sealed record SessionId
{
    public Guid Value { get; }

    private SessionId(Guid value)
    {
        Value = value;
    }

    public static SessionId Create()
    {
        return new SessionId(Guid.NewGuid());
    }

    public static SessionId From(Guid value)
    {
        return new SessionId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    public static implicit operator Guid(SessionId sessionId) => sessionId.Value;
}
