using Backend.Models;

namespace Backend.StaticHelpers;

public static class TemplateQuestions
{
    public static void AddQuickPollQuestions(Guid surveyId, StimmtiDbContext dbContext)
    {
        var question = new FreeTextQuestionTemplate
        {
            Name = "What are your thoughts about the previous topic?",
            OrderNumber = 1,
            SurveyId = surveyId
        };

        dbContext.Add(question);
    }

    public static void AddFeedbackFormQuestions(Guid surveyId, StimmtiDbContext dbContext)
    {
        List<QuestionTemplate> questions = new()
        {
            new WordCloudQuestionTemplate
            {
                Name = "Describe your experience in one word",
                MaxWords = 1,
                OrderNumber = 1,
                SurveyId = surveyId
            },

            new NumberScaleQuestionTemplate
            {
                Name = "How would you rate the overall quality of the activity?",
                MinValue = 0,
                OrderNumber = 2,
                SurveyId = surveyId
            },

            new SingleChoiceQuestionTemplate
            {
                Name = "Did the activity meet your expectations?",
                OrderNumber = 3,
                SurveyId = surveyId
            },

            new FreeTextQuestionTemplate
            {
                Name = "What was your biggest takeaway or most valuable learning?",
                OrderNumber = 4,
                SurveyId = surveyId
            },

            new FreeTextQuestionTemplate
            {
                Name = "Any last feedback/questions to the presentator/s?",
                OrderNumber = 5,
                SurveyId = surveyId
            }
        };

        dbContext.AddRange(questions);

        List<AnswerOption> choiceOptions = new()
        {
            new AnswerOption
            {
                Description = "Exceeded expectations",
                OrderNumber = 1,
                QuestionTemplateId = questions[2].Id
            },

            new AnswerOption
            {
                Description = "Met expectations",
                OrderNumber = 2,
                QuestionTemplateId = questions[2].Id
            },

            new AnswerOption
            {
                Description = "Fell short of expectations",
                OrderNumber = 3,
                QuestionTemplateId = questions[2].Id
            },
        };

        dbContext.AddRange(choiceOptions);
    }

    public static void AddTeamPulseQuestions(Guid surveyId, StimmtiDbContext dbContext)
    {
        List<QuestionTemplate> questions = new()
        {
            new WordCloudQuestionTemplate
            {
                Name = "Describe your current work week in one word",
                MaxWords = 1,
                OrderNumber = 1,
                SurveyId = surveyId
            },

            new SingleChoiceQuestionTemplate
            {
                Name = "How manageable is your current workload?",
                OrderNumber = 2,
                SurveyId = surveyId
            },

            new NumberScaleQuestionTemplate
            {
                Name = "How clear are our team goals and priorities for this sprint?",
                MaxValue = 5,
                OrderNumber = 3,
                SurveyId = surveyId
            },

            new NumberScaleQuestionTemplate
            {
                Name = "How well did our team collaborate this week?",
                OrderNumber = 4,
                SurveyId = surveyId
            },

            new FreeTextQuestionTemplate
            {
                Name = "What obstacles or blockers are currently slowing you down?",
                OrderNumber = 5,
                SurveyId = surveyId
            }
        };

        dbContext.AddRange(questions);

        List<AnswerOption> choiceOptions = new()
        {
            new AnswerOption
            {
                Description = "Too heavy",
                OrderNumber = 1,
                QuestionTemplateId = questions[1].Id
            },

            new AnswerOption
            {
                Description = "Just right",
                OrderNumber = 2,
                QuestionTemplateId = questions[1].Id
            },

            new AnswerOption
            {
                Description = "I have extra capacity",
                OrderNumber = 3,
                QuestionTemplateId = questions[1].Id
            },
        };

        dbContext.AddRange(choiceOptions);
    }
}