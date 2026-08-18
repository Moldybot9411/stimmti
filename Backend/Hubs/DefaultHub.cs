using Backend.Dto;
using Backend.Hubs.Interfaces;
using Backend.Mapper;
using Backend.Models.Enums;
using Backend.Services;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

public class DefaultHub : Hub<ISessionHubClient>, ISessionHub
{
    private readonly StimmtiDbContext _context;
    private readonly IAnswerService _answerService;
    private readonly IParticipantService _participantService;
    private readonly ISessionService _sessionService;

    public DefaultHub(
        StimmtiDbContext context,
        IAnswerService answerService,
        IParticipantService participantService,
        ISessionService sessionService
    )
    {
        _context = context;

        _answerService = answerService;
        _participantService = participantService;
        _sessionService = sessionService;
    }

    public async Task<RestoreStateDto?> JoinSession(JoinSessionDto data)
    {
        var result = await _participantService.JoinSessionAsync(data, Context.ConnectionId, Context.UserIdentifier);
        if (result == null) return null;

        if (result.RestoreStateDto.Role == ParticipantRole.Presenter &&
            result.QuestionId is Guid questionId &&
            result.QuestionType is QuestionTypeEnum questionType)
        {
            var aggregate = await _answerService.GetAggregateResultsAsync(
                questionId,
                questionType
            );

            if (aggregate != null)
            {
                await Clients.Caller.AnswerSubmitted(aggregate);
            }
        }

        return result.RestoreStateDto;
    }

    public async Task LeaveRoom(string roomCode)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);
    }

    public async Task<bool> UpdateParticipantData(ParticipantUpdateDto data)
    {
        var res = await _participantService.UpdateParticipantDataAsync(data);
        return res;
    }

    public async Task<bool> StartSession(string roomCode)
    {
        var res = await _sessionService.StartSessionAsync(roomCode, Context.UserIdentifier);
        return res;
    }

    public async Task<bool> NextQuestion(string roomCode)
    {
        var res = await _sessionService.NextQuestionAsync(roomCode, Context.UserIdentifier);
        return res;
    }

    public async Task<bool> CloseSession(string roomCode)
    {
        var res = await _sessionService.CloseSessionAsync(roomCode, Context.UserIdentifier);
        return res;
    }

    public async Task<bool> SubmitAnswer(SubmitAnswerDto data)
    {
        var submitResult = await _answerService.SubmitAnswerAsync(data);
        if (submitResult == null) return false;

        await _context.SaveChangesAsync();

        var aggregate = await _answerService.GetAggregateResultsAsync(submitResult.QuestionId, submitResult.QuestionType);
        if (aggregate == null) return false;

        await Clients.User(submitResult.OwnerId.ToString()).AnswerSubmitted(aggregate);

        return true;
    }
}