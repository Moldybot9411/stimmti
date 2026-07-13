using Backend.Dto;
using Backend.Models.Enums;
using Backend.Services.Dto;

namespace Backend.Services;

public interface IAnswerService
{
    Task<AnswerServiceSubmitDto?> SubmitAnswerAsync(SubmitAnswerDto data);
    Task<AnswerDisplayDto?> GetAggregateResultsAsync(Guid questionId, QuestionTypeEnum questionType);
}