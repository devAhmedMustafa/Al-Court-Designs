namespace OrdrMate.Models;

public class Item
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.0m;
    public decimal PreperationTime { get; set; } = 0.0m;
    public Category Category { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public Restaurant Restaurant { get; set; }
}