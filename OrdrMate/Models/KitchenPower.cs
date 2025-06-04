namespace OrdrMate.Models;

using OrdrMate.Enums;

public class KitchenPower
{
    public string BranchId { get; set; } = string.Empty;
    public string KitchenId { get; set; } = string.Empty;
    public KitchenStatus Status { get; set; } = KitchenStatus.Active;
    public int Units { get; set; }

    public Branch? Branch { get; set; }
    public Kitchen? Kitchen { get; set; }
}