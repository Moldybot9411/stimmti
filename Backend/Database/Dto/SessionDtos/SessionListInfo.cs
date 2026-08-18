namespace Backend.Dto;

public class SessionListInfoDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required DateTimeOffset OpenedAt { get; set; }
    public required int ParticipantCount { get; set; }
}