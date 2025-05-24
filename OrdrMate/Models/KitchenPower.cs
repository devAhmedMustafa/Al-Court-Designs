namespace OrdrMate.Models;

public enum KitchenStatus
{
    Active=0,
    Inactive=1,
    Maintenance=2
}

public class KitchenPower
{
    public string BranchId { get; set; } = string.Empty;
    public string KitchenId { get; set; } = string.Empty;
    public KitchenStatus Status { get; set; } = KitchenStatus.Active;
    public int Units { get; set; }

    public Branch? Branch { get; set; }
    public Kitchen? Kitchen { get; set; }
}