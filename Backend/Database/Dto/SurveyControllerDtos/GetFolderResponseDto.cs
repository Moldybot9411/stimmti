namespace Backend.Dto;

public class GetFolderResponseDto
{
    public required Guid FolderId { get; set; }
    public required string Name { get; set; }
    public required List<GetSurveyResponseDto> Surveys { get; set; }
}