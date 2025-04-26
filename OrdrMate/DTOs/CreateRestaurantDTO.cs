using System.ComponentModel.DataAnnotations;

namespace OrdrMate.DTOs;

public class CreateRestaurantDTO {
    [Required] public string Name {get; set;}
    public string Phone{get; set;}
    public string Email{get; set;}
    public string ManagerUsername{get; set;}
}