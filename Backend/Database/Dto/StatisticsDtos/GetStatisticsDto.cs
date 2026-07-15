namespace Backend.Dto;

public class GetStatisticsDto
{
    public required int SurveyCount { get; set; }
    public required int SurveyDelta { get; set; }
    public required int SessionCount { get; set; }
    public required int SessionDelta { get; set; }
    public required int ParticipantCount { get; set; }
    public required int ParticipantDelta { get; set; }

    public List<SurveyStatisticsDto> SurveyStatistics { get; } = new();
    public List<SessionStatisticsDto> SessionStatistics { get; } = new();
}