namespace OrdrMate.DTOs;

public class AddBranchRequestDto
{
    public float Lantitude { get; set; }
    public float Longitude { get; set; }
    public string BranchAddress { get; set; }
    public string BranchPhoneNumber { get; set; }
    public string RestaurantId { get; set; }
}