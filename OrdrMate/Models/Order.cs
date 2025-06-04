using OrdrMate.Enums;

namespace OrdrMate.Models;

public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string BranchId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public TimeOnly OrderTime { get; set; } = TimeOnly.FromDateTime(DateTime.UtcNow);
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderType OrderType { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Queued;
    public decimal TotalAmount { get; set; } = 0.0m;
    public bool IsPaid { get; set; } = false;
    public Branch? Branch { get; set; }
    public User? Customer { get; set; }
    public Payment? Payment { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
}