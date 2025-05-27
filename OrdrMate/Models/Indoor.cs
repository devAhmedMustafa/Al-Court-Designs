namespace OrdrMate.Models;

public class Indoor
{
    public required int TableNumber { get; set; }
    public required string BranchId { get; set; }
    public required string OrderId { get; set; }
    public Order Order { get; set; }
    public Branch Branch { get; set; }
}