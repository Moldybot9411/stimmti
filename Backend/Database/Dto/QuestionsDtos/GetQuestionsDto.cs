using Backend.Models.Enums;

public class GetQuestionTemplateResponseDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int? OrderNumber { get; set; }
    public bool? IsArchived { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? MaxWords { get; set; }
    public Guid SurveyId { get; set; }
    public QuestionTypeEnum QuestionType { get; set; }
    public string[] Answers { get; set; } = Array.Empty<string>();

}