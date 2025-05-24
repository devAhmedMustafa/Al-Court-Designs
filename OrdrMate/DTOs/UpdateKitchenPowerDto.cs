using OrdrMate.Models;

namespace OrdrMate.DTOs;

public class UpdateKitchenPowerDto
{
    public required int Units { get; set; }
    public required KitchenStatus Status { get; set; }
}