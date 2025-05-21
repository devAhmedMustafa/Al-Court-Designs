namespace OrdrMate.Models;

public class BranchRequest
{
    public string Id { get; set; }
    public float Lantitude { get; set; }
    public float Longitude { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; }
}