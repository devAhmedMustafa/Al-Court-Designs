namespace OrdrMate.DTOs;

public class TableDto
{
    public int TableNumber { get; set; }
    public int Seats { get; set; }
    public string BranchId { get; set; } = string.Empty;
}