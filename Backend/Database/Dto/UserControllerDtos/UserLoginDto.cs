using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class UserLoginDto
{
    [MaxLength(64)]
    public required string Password { get; set; }

    [MaxLength(20)]
    public required string Username { get; set; }
    public bool StaySignedIn { get; set; } = false;
}