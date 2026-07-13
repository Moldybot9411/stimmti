namespace Backend.Dto;

public class QuestionDto
{
    public required QuestionTemplateDto QuestionTemplateDto { get; set; }
    public required AnswerDisplayDto AnswerDisplayDto { get; set; }
}