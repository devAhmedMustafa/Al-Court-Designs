using OrdrMate.Enums;

namespace OrdrMate.DTOs.User;

public class ManagerDTO {
    public required string Id { get; set; }
    public required string Username { get; set; }
    public UserRole Role { get; set; }
}