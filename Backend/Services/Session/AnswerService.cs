using Backend.Dto;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services.Dto;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class AnswerService : IAnswerService
{
    private readonly StimmtiDbContext _context;


    public AnswerService(StimmtiDbContext context)
    {
        _context = context;
    }

    public async Task<AnswerServiceSubmitDto?> SubmitAnswerAsync(SubmitAnswerDto data)
    {
        var session = await _context.Sessions
            .Include(x => x.Survey)
            .Include(x => x.CurrentQuestion)
                .ThenInclude(x => (x!.QuestionTemplate as ChoiceQuestionTemplate)!.AnswerOptions)
            .Include(x => x.CurrentQuestion)
                .ThenInclude(x => x.QuestionTemplate)
            .FirstOrDefaultAsync(x => x.RoomCode == data.RoomCode && x.RoomActive == true);

        if (session == null) return null;
        if (session.CurrentQuestion == null || session.CurrentQuestion.QuestionTemplate == null) return null;

        var participant = await _context.AnonymousUsers
            .Include(x => x.Answers)
            .FirstOrDefaultAsync(x => x.Id == data.AnonymousUserId && x.SessionId == session.Id);
        if (participant == null) return null;
        if (participant.AnsweredThisRound) return null;

        var template = session.CurrentQuestion.QuestionTemplate;
        var questionId = session.CurrentQuestion.Id;
        var userId = participant.Id;

        switch (template.QuestionType)
        {
            case QuestionTypeEnum.SingleChoice:
                var singleChoiceDto = data.AnswerOptions.FirstOrDefault();
                if (singleChoiceDto == null) return null;

                var singleChoiceTemplate = (ChoiceQuestionTemplate)template;
                if (!singleChoiceTemplate.AnswerOptions.Any(x => x.Id == singleChoiceDto.Id)) return null;

                _context.Add(new SingleChoiceAnswer
                {
                    AnonymousUserId = userId,
                    QuestionId = questionId,
                    AnswerOptionId = singleChoiceDto.Id
                });
                break;

            case QuestionTypeEnum.MultipleChoice:
                if (!data.AnswerOptions.Any()) return null;

                var multiChoiceTemplate = (ChoiceQuestionTemplate)template;
                var validOptionIds = multiChoiceTemplate.AnswerOptions.Select(x => x.Id).ToList();

                foreach (var opt in data.AnswerOptions)
                {
                    if (!validOptionIds.Contains(opt.Id)) return null;

                    _context.Add(new MultipleChoiceAnswer
                    {
                        AnonymousUserId = userId,
                        QuestionId = questionId,
                        AnswerOptionId = opt.Id
                    });
                }

                break;
            case QuestionTypeEnum.FreeText:
                if (string.IsNullOrWhiteSpace(data.Text)) return null;
                if (data.Text.Length > 256) return null;

                _context.Add(new FreeTextAnswer
                {
                    AnonymousUserId = userId,
                    QuestionId = questionId,
                    Text = data.Text.Trim()
                });

                break;
            case QuestionTypeEnum.WordCloud:
                if (data.WordCloudAnswers == null || data.WordCloudAnswers.Count == 0) return null;

                var wordCloudTemplate = (WordCloudQuestionTemplate)template;
                if (data.WordCloudAnswers.Count > wordCloudTemplate.MaxWords) return null;

                if (data.WordCloudAnswers.Any(x => string.IsNullOrWhiteSpace(x) || x.Trim().Split(" ").Length > 1 || x.Length > 64)) return null;

                foreach (var answer in data.WordCloudAnswers)
                {
                    _context.Add(new WordCloudAnswer
                    {
                        AnonymousUserId = userId,
                        QuestionId = questionId,
                        Text = answer.Trim()
                    });
                }

                break;
            case QuestionTypeEnum.NumberScale:
                if (!data.Value.HasValue) return null;

                var numberTemplate = (NumberScaleQuestionTemplate)template;
                if (data.Value.Value < numberTemplate.MinValue || data.Value.Value > numberTemplate.MaxValue) return null;

                _context.Add(new NumberScaleAnswer
                {
                    AnonymousUserId = userId,
                    QuestionId = questionId,
                    Value = data.Value.Value
                });

                break;

            default:
                return null;
        }

        participant.AnsweredThisRound = true;

        return new AnswerServiceSubmitDto
        {
            QuestionId = questionId,
            OwnerId = session.Survey!.OwnerId,
            QuestionType = template.QuestionType
        };
    }

    public async Task<AnswerDisplayDto?> GetAggregateResultsAsync(Guid questionId, QuestionTypeEnum questionType)
    {
        var displayDto = new AnswerDisplayDto();

        switch (questionType)
        {
            case QuestionTypeEnum.SingleChoice:
            case QuestionTypeEnum.MultipleChoice:
                displayDto.ChoiceResults = await _context.Set<ChoiceAnswer>()
                    .Include(a => a.AnswerOption)
                    .Where(a => a.QuestionId == questionId)
                    .GroupBy(a => a.AnswerOptionId)
                    .Select(g => new ChoiceResultDto
                    {
                        AnswerOption = new AnswerOptionDto
                        {
                            Id = g.Key,
                            Description = g.First().AnswerOption!.Description
                        },
                        Count = g.Count()
                    })
                    .ToListAsync();
                break;

            case QuestionTypeEnum.WordCloud:
                displayDto.WordCloudResults = await _context.Set<WordCloudAnswer>()
                    .Where(a => a.QuestionId == questionId)
                    .GroupBy(a => a.Text.ToLower())
                    .Select(g => new WordCloudResultDto
                    {
                        Text = g.First().Text,
                        Count = g.Count()
                    })
                    .ToListAsync();
                break;
            case QuestionTypeEnum.FreeText:
                displayDto.FreeTextResults = await _context.Set<FreeTextAnswer>()
                    .Where(a => a.QuestionId == questionId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Select(g => new FreeTextResultDto
                    {
                        Id = g.Id,
                        Text = g.Text,
                    })
                    .ToListAsync();
                break;

            case QuestionTypeEnum.NumberScale:
                displayDto.NumberResults = await _context.Set<NumberScaleAnswer>()
                    .Where(a => a.QuestionId == questionId)
                    .GroupBy(a => a.Value)
                    .Select(g => new NumberResultDto
                    {
                        Value = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();
                break;
        }

        displayDto.TotalParticipantsAnswered = await _context.Set<Answer>()
            .Where(a => a.QuestionId == questionId)
            .Select(a => a.AnonymousUserId)
            .Distinct()
            .CountAsync();

        return displayDto;
    }
}