namespace OrdrMate.DTOs.Kitchen;

public class AddKitchenPowerDto
{
    public required string BranchId { get; set; }
    public required string KitchenId { get; set; }
    public required int Units { get; set; }
}