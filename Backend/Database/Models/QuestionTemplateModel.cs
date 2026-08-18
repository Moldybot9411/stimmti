using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Models.Enums;

namespace Backend.Models;

public abstract class QuestionTemplate
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(255)]
    public required string Name { get; set; }

    [MaxLength(2048)]
    public string? Description { get; set; }

    public required Guid SurveyId { get; set; }
    public Survey? Survey { get; set; }

    [NotMapped]
    public QuestionTypeEnum QuestionType => this switch
    {
        SingleChoiceQuestionTemplate => QuestionTypeEnum.SingleChoice,
        MultipleChoiceQuestionTemplate => QuestionTypeEnum.MultipleChoice,
        WordCloudQuestionTemplate => QuestionTypeEnum.WordCloud,
        FreeTextQuestionTemplate => QuestionTypeEnum.FreeText,
        NumberScaleQuestionTemplate => QuestionTypeEnum.NumberScale,
        _ => throw new InvalidOperationException($"Unknown QuestionTemplate type: {GetType().Name}")
    };

    public List<Question> Questions { get; } = new List<Question>();

    public required int OrderNumber { get; set; }
    public bool IsArchived { get; set; } = false;
}

public abstract class ChoiceQuestionTemplate : QuestionTemplate
{
    public List<AnswerOption> AnswerOptions { get; } = new List<AnswerOption>();
}

public class SingleChoiceQuestionTemplate : ChoiceQuestionTemplate { }

public class MultipleChoiceQuestionTemplate : ChoiceQuestionTemplate { }

public class WordCloudQuestionTemplate : QuestionTemplate
{
    public required int MaxWords { get; set; } = 3;
}

public class FreeTextQuestionTemplate : QuestionTemplate { }

public class NumberScaleQuestionTemplate : QuestionTemplate
{
    public int MinValue { get; set; } = 1;
    public int MaxValue { get; set; } = 10;
}