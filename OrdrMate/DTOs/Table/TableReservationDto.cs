namespace OrdrMate.DTOs.Table;

using OrdrMate.Models;

public class TableReservationDto
{
    public required int TableNumber { get; set; }
    public Order? Order { get; set; }
}