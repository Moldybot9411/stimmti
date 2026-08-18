using Backend.Hubs;
using Backend.Hubs.Interfaces;
using Backend.Mapper;
using Backend.Models;
using Backend.Models.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class SessionService : ISessionService
{
    private readonly StimmtiDbContext _context;
    private readonly IApiMapper _mapper;
    private readonly IHubContext<DefaultHub, ISessionHubClient> _hubContext;

    public SessionService(
        StimmtiDbContext context,
        IApiMapper mapper, IHubContext<DefaultHub,
        ISessionHubClient> hubContext
    )
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<bool> StartSessionAsync(string roomCode, string? userIdentifier)
    {
        var session = await _context.Sessions
            .Include(x => x.AnonymousParticipants)
            .Include(x => x.Survey)
            .Include(x => x.Questions.OrderBy(y => y.QuestionTemplate!.OrderNumber))
                .ThenInclude(x => x.QuestionTemplate)
            .Include(x => x.Questions)
                .ThenInclude(x => (x.QuestionTemplate as ChoiceQuestionTemplate)!.AnswerOptions.OrderBy(y => y.OrderNumber))
            .FirstOrDefaultAsync(x => x.RoomCode == roomCode && x.RoomActive == true);
        if (session == null) return false;

        if (userIdentifier == null || session.Survey!.OwnerId.ToString() != userIdentifier) return false;

        if (session.Questions.Count == 0) return false;
        if (session.AnonymousParticipants.Count == 0) return false;

        await _hubContext.Clients.Group(roomCode).SessionStateChanged(SessionState.Loading);

        session.CurrentState = SessionState.Loading;
        await _context.SaveChangesAsync();

        var firstQuestion = session.Questions.First();
        await _hubContext.Clients.Group(roomCode).QuestionChanged(_mapper.MapToQuestionTemplateDto(firstQuestion.QuestionTemplate!));

        session.CurrentState = SessionState.Question;
        session.CurrentQuestionId = firstQuestion.Id;
        await _context.SaveChangesAsync();

        await Task.Delay(3000);

        await _hubContext.Clients.Group(roomCode).SessionStateChanged(SessionState.Question);

        return true;
    }

    public async Task<bool> NextQuestionAsync(string roomCode, string? userIdentifier)
    {
        var session = await _context.Sessions
            .Include(x => x.Survey)
            .Include(x => x.AnonymousParticipants)
            .Include(x => x.Questions.OrderBy(y => y.QuestionTemplate!.OrderNumber))
                .ThenInclude(x => x.QuestionTemplate)
            .Include(x => x.Questions)
                .ThenInclude(x => (x.QuestionTemplate as ChoiceQuestionTemplate)!.AnswerOptions.OrderBy(y => y.OrderNumber))
            .FirstOrDefaultAsync(x => x.RoomCode == roomCode && x.RoomActive == true);
        if (session == null || session.Questions.Count == 0) return false;

        if (userIdentifier == null || session.Survey!.OwnerId.ToString() != userIdentifier) return false;
        if (session.CurrentQuestion == null) return false;

        var currentIndex = session.Questions.FindIndex(q => q.Id == session.CurrentQuestionId);
        if (currentIndex == -1) return false;

        // Last question
        if (currentIndex == session.Questions.Count - 1)
        {
            await _hubContext.Clients.Group(roomCode).SessionStateChanged(SessionState.Finished);

            session.CurrentState = SessionState.Finished;
            session.CurrentQuestionId = null;
            await _context.SaveChangesAsync();

            return true;
        }

        // Load next question
        await _hubContext.Clients.Group(roomCode).SessionStateChanged(SessionState.Loading);

        var nextQuestion = session.Questions[currentIndex + 1];

        foreach (var participant in session.AnonymousParticipants)
        {
            participant.AnsweredThisRound = false;
        }

        session.CurrentQuestionId = nextQuestion.Id;
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group(roomCode).QuestionChanged(_mapper.MapToQuestionTemplateDto(nextQuestion.QuestionTemplate!));

        await Task.Delay(1000);

        await _hubContext.Clients.Group(roomCode).SessionStateChanged(SessionState.Question);

        return true;
    }

    public async Task<bool> CloseSessionAsync(string roomCode, string? userIdentifier)
    {
        var session = await _context.Sessions
            .Include(x => x.Survey)
            .FirstOrDefaultAsync(x => x.RoomCode == roomCode && x.RoomActive == true);
        if (session == null) return false;

        if (userIdentifier == null || session.Survey!.OwnerId.ToString() != userIdentifier) return false;

        session.RoomActive = false;
        session.CurrentQuestion = null;
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group(roomCode).SessionClosed();

        return true;
    }
}