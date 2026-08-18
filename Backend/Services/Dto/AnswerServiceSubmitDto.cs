using Backend.Models.Enums;

namespace Backend.Services.Dto;

public class AnswerServiceSubmitDto
{
    public required Guid QuestionId { get; set; }
    public required Guid OwnerId { get; set; }
    public required QuestionTypeEnum QuestionType { get; set; }
}