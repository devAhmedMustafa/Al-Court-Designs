namespace OrdrMate.DTOs.Order;

public class OrderWaitingTimesDto
{
    public decimal AverageWaitingTime { get; set; } = 0.0m;
    public decimal MinWaitingTime { get; set; } = 0.0m;
    public decimal MaxWaitingTime { get; set; } = 0.0m;
}