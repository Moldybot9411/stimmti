namespace Backend.Dto;

public class PaginatedSessionListDto
{
    public required int SessionCount { get; set; }
    public List<SessionListInfoDto> sessionListInfo { get; set; } = new();
}