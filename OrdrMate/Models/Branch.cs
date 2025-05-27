namespace OrdrMate.Models;

public class Branch
{
    public string Id { get; set; }
    public float Lantitude { get; set; }
    public float Longitude { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public string RestaurantId { get; set; }
    public string BranchManagerId { get; set; }
    public User BranchManager { get; set; }
    public Restaurant Restaurant { get; set; }
    public ICollection<Table> Tables { get; set; }
    public ICollection<Order> Orders { get; set; }
    
}