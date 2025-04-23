using OrdrMate.Models;

namespace OrdrMate.DTOs;

public class ManagerDTO {
    public required string Id { get; set; }
    public required string Username { get; set; }
    public ManagerRole Role { get; set; }
}