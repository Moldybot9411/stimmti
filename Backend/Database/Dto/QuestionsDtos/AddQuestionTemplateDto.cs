using Backend.Dto;
using Backend.Models.Enums;

public class CreateQuestionTemplateDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    public required Guid SurveyId { get; set; }
    public int? OrderNumber { get; set; }
    public required bool IsArchived { get; set; }
    public required QuestionTypeEnum QuestionType { get; set; }
    public string[] Answers { get; set; } = Array.Empty<string>();
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? MaxWords { get; set; }

}