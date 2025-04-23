namespace OrdrMate.DTOs;

public class ManagerDTO {
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Username { get; set; }
    public required string Password { get; set; }
}