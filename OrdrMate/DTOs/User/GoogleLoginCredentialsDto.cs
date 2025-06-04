namespace OrdrMate.DTOs.User;

public class GoogleLoginCredentialsDto
{
    public required string UserId { get; set; }
    public required string Token { get; set; }
    public required string Email { get; set; }
}