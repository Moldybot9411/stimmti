namespace Backend.Dto;

public class SessionStatisticsDto
{
    public required string Name { get; set; }
    public required int ParticipantCount { get; set; }
    public required DateTimeOffset OpenedAt { get; set; }
}