using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class UserRegisterDto
{
    [MaxLength(20)]
    public required string Username { get; set; }

    [MaxLength(64)]
    public required string Password { get; set; }
}