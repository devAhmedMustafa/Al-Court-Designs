using OrdrMate.Enums;

namespace OrdrMate.DTOs.Kitchen;

public class KitchenPowerDto
{
    public string KitchenId { get; set; } = string.Empty;
    public string KitchenName { get; set; } = string.Empty;
    public string BranchId { get; set; } = string.Empty;
    public int Units { get; set; } = 1;
    public KitchenStatus Status { get; set; }
}