namespace OrdrMate.Models;

public class Manager
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Username { get; set; }
    public required string Password { get; set; }
    public ManagerRole Role { get; set; } = ManagerRole.BranchManager;
}