using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Question
{
    [Key]
    public Guid Id { get; set; }

    public required Guid SessionId { get; set; }
    public Session? Session { get; set; }

    public required Guid QuestionTemplateId { get; set; }
    public QuestionTemplate? QuestionTemplate { get; set; }

    public List<Answer> Answers { get; set; } = new List<Answer>();
}