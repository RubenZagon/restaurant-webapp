namespace RestaurantApp.Domain.ValueObjects;

public enum OrderStatus
{
    Draft = 0,        // Order is being created
    Confirmed = 1,    // Order confirmed, sent to kitchen
    Preparing = 2,    // Kitchen is preparing
    Ready = 3,        // Order ready for delivery
    Delivered = 4,    // Order delivered to table
    Cancelled = 5     // Order cancelled
}
