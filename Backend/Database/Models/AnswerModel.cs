using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public abstract class Answer
{
    [Key]
    public Guid Id { get; set; }

    public required Guid AnonymousUserId { get; set; }
    public AnonymousUser? AnonymousUser { get; set; }

    public required Guid QuestionId { get; set; }
    public Question? Question { get; set; }
}

public abstract class ChoiceAnswer : Answer
{
    public required Guid AnswerOptionId { get; set; }
    public AnswerOption? AnswerOption { get; set; }
}

public class SingleChoiceAnswer : ChoiceAnswer { }

public class MultipleChoiceAnswer : ChoiceAnswer { }

public class WordCloudAnswer : Answer
{
    [MaxLength(64)]
    public required string Text { get; set; }
}

public class FreeTextAnswer : Answer
{
    [MaxLength(256)]
    public required string Text { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class NumberScaleAnswer : Answer
{
    public required int Value { get; set; }
}