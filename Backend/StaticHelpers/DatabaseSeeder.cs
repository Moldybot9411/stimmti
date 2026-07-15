using Backend.Models;
using Bogus;

namespace Backend.StaticHelpers;

public static class DatabaseSeeder
{
    // Password: Tester42
    static string defaultPasswordHash = "AQAAAAIAAYagAAAAEKuQEABVjWTNT9YVi8Q3T93WvQlyWYNTr/Tp3VNQWwwmOQDAF/dmxEn0ZQefTX2hEg==";

    public static void SeedUsers(StimmtiDbContext dbContext)
    {
        if (dbContext.Users.Any()) return;

        AddAdminTestData(dbContext);

        var userFaker = new Faker<User>("de")
            .RuleFor(c => c.Id, f => Guid.Empty)
            .RuleFor(c => c.UserName, f => "Bogus" + (f.IndexFaker + 1))
            .RuleFor(c => c.NormalizedUserName, (f, u) => u.UserName!.ToUpper())
            .RuleFor(c => c.Email, f => $"bogus{f.IndexFaker}@example.com")
            .RuleFor(c => c.NormalizedEmail, (f, u) => u.Email!.ToUpper())
            .RuleFor(c => c.PasswordHash, f => defaultPasswordHash)
            .RuleFor(c => c.SecurityStamp, f => Guid.NewGuid().ToString("D"))
            .RuleFor(u => u.ConcurrencyStamp, f => Guid.NewGuid().ToString("D"));

        var dummyUsers = userFaker.Generate(20);

        dbContext.Users.AddRange(dummyUsers);
        dbContext.SaveChanges();
    }

    private static void AddAdminTestData(StimmtiDbContext dbContext)
    {
        var adminUser = new User
        {
            UserName = "Admin",
            NormalizedUserName = "ADMIN",
            Email = "a@a.a",
            NormalizedEmail = "A@A.A",
            PasswordHash = defaultPasswordHash,
            SecurityStamp = Guid.NewGuid().ToString("D"),
            ConcurrencyStamp = Guid.NewGuid().ToString("D")
        };

        dbContext.Users.Add(adminUser);

        for (int i = 0; i < 30; i++)
        {

            var testSurvey = new Survey
            {
                Title = $"Test Survey {i + 1}",
                OwnerId = adminUser.Id
            };

            dbContext.Surveys.Add(testSurvey);

            var testQuestions = new List<QuestionTemplate>{
                new SingleChoiceQuestionTemplate
                {
                    Name = "Question 1 (SingleChoice)",
                    Description = "The user can submit one of many answer options",
                    SurveyId = testSurvey.Id,
                    OrderNumber = 1,
                },
                new MultipleChoiceQuestionTemplate
                {
                    Name = "Question 2 (MultipleChoice)",
                    Description = "The user can submit multiple of many answer options",
                    SurveyId = testSurvey.Id,
                    OrderNumber = 2
                },
                new WordCloudQuestionTemplate
                {
                    Name = "Question 3 (WordCloud)",
                    Description = "The user can submit a specified number of words which get grouped and displayed in a cloud",
                    SurveyId = testSurvey.Id,
                    OrderNumber = 3,
                    MaxWords = 3
                },
                new FreeTextQuestionTemplate
                {
                    Name = "Question 4 (FreeText)",
                    Description = "The user can submit any text",
                    SurveyId = testSurvey.Id,
                    OrderNumber = 4
                },
                new NumberScaleQuestionTemplate
                {
                    Name = "Question 5 (NumberScale)",
                    Description = "The user can vote on a scale from a specified minimum to a specified maximum value",
                    SurveyId = testSurvey.Id,
                    OrderNumber = 5
                },
            };

            dbContext.QuestionTemplates.AddRange(testQuestions);

            AddAnswerOptions(dbContext, testQuestions[0].Id);
            AddAnswerOptions(dbContext, testQuestions[1].Id);
        }


        var folder = new Folder
        {
            Name = "Test Folder",
            OwnerId = adminUser.Id
        };

        dbContext.Folders.Add(folder);

        var surveyInFolder = new Survey
        {
            Title = "Survey in Folder",
            OwnerId = adminUser.Id,
            FolderId = folder.Id
        };

        dbContext.Surveys.Add(surveyInFolder);
    }

    private static void AddAnswerOptions(StimmtiDbContext dbContext, Guid questionTemplateId)
    {
        var answerOptions = new List<AnswerOption>
        {
            new AnswerOption
            {
                OrderNumber = 1,
                Description = "Good (Slot 1)",
                QuestionTemplateId = questionTemplateId
            },
            new AnswerOption
            {
                OrderNumber = 2,
                Description = "Bad (Slot 2)",
                QuestionTemplateId = questionTemplateId
            },
        };

        dbContext.AnswerOptions.AddRange(answerOptions);
    }
}