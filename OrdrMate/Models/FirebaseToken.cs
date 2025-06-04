namespace OrdrMate.Models;

public class FirebaseToken
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public User? User { get; set; }
}