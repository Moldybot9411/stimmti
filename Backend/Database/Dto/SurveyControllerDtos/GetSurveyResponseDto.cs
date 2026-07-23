namespace Backend.Dto;

public class GetSurveyResponseDto
{
    public required Guid SurveyId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid? FolderId { get; set; }
    public bool IsFavorite { get; set; }
}