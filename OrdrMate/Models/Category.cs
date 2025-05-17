namespace OrdrMate.Models;

public class Category
{
    public string Name { get; set; } = string.Empty;
    public string RestaurantId { get; set; } = string.Empty;
    public Restaurant Restaurant { get; set; }
    public ICollection<Item> Items { get; set; } = new List<Item>();
}