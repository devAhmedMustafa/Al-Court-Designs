using System.ComponentModel.DataAnnotations;

namespace OrdrMate.DTOs.Restaurant;

public class CreateRestaurantDto {
    [Required] public string Name {get; set;}
    public string Phone{get; set;}
    public string Email{get; set;}
    [Required] public string ManagerUsername{get; set;}
}