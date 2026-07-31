using System.ComponentModel.DataAnnotations;
using Backend.Dto;
using Backend.Models.Enums;

public class CreateQuestionTemplateDto
{
    [MaxLength(255)]
    public required string Name { get; set; }
    [MaxLength(2048)]
    public required string Description { get; set; }

    public required Guid SurveyId { get; set; }
    public int? OrderNumber { get; set; }
    public required bool IsArchived { get; set; }
    public required QuestionTypeEnum QuestionType { get; set; }
    public List<string> Answers { get; set; } = new();
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? MaxWords { get; set; }

}