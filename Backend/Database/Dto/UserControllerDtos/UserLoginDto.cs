namespace Backend.Dto;

public class UserLoginDto
{
    public required string Password { get; set; }
    public required string Username { get; set; }
    public bool StaySignedIn { get; set; } = false;
}