using System.ComponentModel.DataAnnotations;

namespace OrdrMate.DTOs.User;

public class CreateManagerDTO {
    [Required, MinLength(3)] public string Username {get; set;}
    [Required, MinLength(8)] public string Password {get; set;}
}