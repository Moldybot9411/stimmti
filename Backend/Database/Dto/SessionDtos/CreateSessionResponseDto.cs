namespace Backend.Dto;

public class CreateSessionResponseDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string RoomCode { get; set; }
}