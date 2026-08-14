
using Backend.Models;
using Backend.Models.Enums;


public static class QuestionTemplateCloner
{
    public static QuestionTemplate CloneQuestionTemplate(QuestionTemplate source)
    {
        QuestionTemplate clonedTemplate = source.QuestionType switch
        {
            QuestionTypeEnum.SingleChoice => new SingleChoiceQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = source.Name,
                Description = source.Description,
                SurveyId = source.SurveyId,
                OrderNumber = source.OrderNumber,
                IsArchived = source.IsArchived,
            },
            QuestionTypeEnum.MultipleChoice => new MultipleChoiceQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = source.Name,
                Description = source.Description,
                SurveyId = source.SurveyId,
                OrderNumber = source.OrderNumber,
                IsArchived = source.IsArchived,
            },
            QuestionTypeEnum.WordCloud => new WordCloudQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = source.Name,
                Description = source.Description,
                SurveyId = source.SurveyId,
                OrderNumber = source.OrderNumber,
                IsArchived = source.IsArchived,
                MaxWords = ((WordCloudQuestionTemplate)source).MaxWords,
            },
            QuestionTypeEnum.FreeText => new FreeTextQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = source.Name,
                Description = source.Description,
                SurveyId = source.SurveyId,
                OrderNumber = source.OrderNumber,
                IsArchived = source.IsArchived,
            },
            QuestionTypeEnum.NumberScale => new NumberScaleQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = source.Name,
                Description = source.Description,
                SurveyId = source.SurveyId,
                OrderNumber = source.OrderNumber,
                IsArchived = source.IsArchived,
                MinValue = ((NumberScaleQuestionTemplate)source).MinValue,
                MaxValue = ((NumberScaleQuestionTemplate)source).MaxValue,
            },
            _ => throw new InvalidOperationException($"Unsupported QuestionTemplate type: {source.GetType().Name}"),
        };

        if (source is ChoiceQuestionTemplate sourceChoiceTemplate && clonedTemplate is ChoiceQuestionTemplate clonedChoiceTemplate)
        {
            clonedChoiceTemplate.AnswerOptions.AddRange(
                sourceChoiceTemplate.AnswerOptions
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => new AnswerOption
                    {
                        OrderNumber = x.OrderNumber,
                        Description = x.Description,
                        QuestionTemplateId = clonedChoiceTemplate.Id,
                    }).ToList()
            );
        }

        return clonedTemplate;
    }
}