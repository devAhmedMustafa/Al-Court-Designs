using OrdrMate.Enums;

namespace OrdrMate.DTOs;

public class ManagerDTO {
    public required string Id { get; set; }
    public required string Username { get; set; }
    public UserRole Role { get; set; }
}