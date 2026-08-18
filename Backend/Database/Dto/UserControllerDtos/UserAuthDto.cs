namespace Backend.Dto;

public class UserAuthDto
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string DisplayName { get; set; }
    public string? ProfilePictureUrl { get; set; }
}