namespace OrdrMate.Models;

public class OrderItem
{
    public string OrderId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal Price { get; set; } = 0.0m;

    public Order? Order { get; set; }
    public Item? Item { get; set; }
}