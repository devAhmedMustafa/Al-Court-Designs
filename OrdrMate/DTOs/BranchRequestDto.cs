namespace OrdrMate.DTOs;

public class BranchRequestDto
{
    public string BranchRequestId { get; set; }
    public float Lantitude { get; set; }
    public float Longitude { get; set; }
    public string BranchAddress { get; set; }
    public string BranchPhoneNumber { get; set; }
    public string RestaurantName { get; set; }
}