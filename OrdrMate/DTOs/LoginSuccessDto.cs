namespace OrdrMate.DTOs
{
    public class LoginSuccessDto
    {
        public string Token { get; set; }
        public string Role { get; set; }
        public string RestaurantId { get; set; }
        public string BranchId { get; set; }
    }
}