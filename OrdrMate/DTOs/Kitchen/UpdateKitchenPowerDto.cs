using OrdrMate.Enums;

namespace OrdrMate.DTOs.Kitchen;

public class UpdateKitchenPowerDto
{
    public required int Units { get; set; }
    public required KitchenStatus Status { get; set; }
}