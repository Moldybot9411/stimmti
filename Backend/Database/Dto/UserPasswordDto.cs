using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;


public class UserPasswordDto
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;
}