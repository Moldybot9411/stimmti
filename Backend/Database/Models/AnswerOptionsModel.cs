using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class AnswerOption
{
    [Key]
    public Guid Id { get; set; }

    public required int OrderNumber { get; set; }

    [MaxLength(2048)]
    public required string Description { get; set; }

    public required Guid QuestionTemplateId { get; set; }
    public ChoiceQuestionTemplate? QuestionTemplate { get; set; }

    public bool IsArchived { get; set; } = false;
    public List<ChoiceAnswer> Answers { get; set; } = new List<ChoiceAnswer>();
}