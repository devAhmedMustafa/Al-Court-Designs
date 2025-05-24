namespace OrdrMate.DTOs;

public class AddKitchenDto
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public required string RestaurantId { get; set; }
}