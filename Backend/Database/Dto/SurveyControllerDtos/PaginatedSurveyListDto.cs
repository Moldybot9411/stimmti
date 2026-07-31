namespace Backend.Dto;

public class PaginatedSurveyListDto
{
    public required int SurveyCount { get; set; }
    public List<GetSurveyResponseDto> SurveyListInfo { get; set; } = new();
}