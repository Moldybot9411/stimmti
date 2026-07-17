namespace Backend.Dto;

public class GetOpenSessionsDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string RoomCode { get; set; }
    public required DateTimeOffset OpenedAt { get; set; }
}