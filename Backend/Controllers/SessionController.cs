using Backend.Dto;
using Backend.Hubs;
using Backend.Hubs.Interfaces;
using Backend.Mapper;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/[controller]")]
public class SessionController : ControllerBase
{
    private readonly StimmtiDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserController> _logger;
    private readonly IApiMapper _mapper;
    private readonly IHubContext<DefaultHub, ISessionHubClient> _hubContext;
    private readonly IAnswerService _answerService;

    public SessionController(
        StimmtiDbContext context,
        UserManager<User> userManager,
        ILogger<UserController> logger,
        IApiMapper mapper,
        IHubContext<DefaultHub, ISessionHubClient> hubContext,
        IAnswerService answerService
    )
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _mapper = mapper;
        _hubContext = hubContext;
        _answerService = answerService;
    }

    [HttpPost("createSession")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(CreateSessionResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSession(CreateSessionDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var survey = await _context.Surveys
            .Include(x => x.QuestionTemplates)
            .FirstOrDefaultAsync(x => x.Id == data.SurveyId);

        if (survey == null)
            return BadRequest(new ProblemDetails { Title = "Survey not found", Detail = $"Survey with ID {data.SurveyId} doesn't exist" });

        if (survey.QuestionTemplates.Count == 0)
            return BadRequest(new ProblemDetails { Title = "Survey has no questions", Detail = $"Survey with ID {data.SurveyId} has no questions" });

        if (survey.OwnerId != user.Id)
            return Forbid();

        var possibleCharacters = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Removed: O/0, I/1
        var roomCode = "";

        var rng = new Random();

        var maxRetries = 3;
        for (int i = 0; i < maxRetries; i++)
        {
            roomCode = "";
            for (int j = 0; j < 6; j++)
            {
                roomCode += possibleCharacters[rng.Next(possibleCharacters.Length)];
                if (j == 2) roomCode += "-";
            }

            var existingSession = await _context.Sessions.FirstOrDefaultAsync(x => x.RoomCode == roomCode && x.RoomActive == true);
            if (existingSession == null) break;
            if (i == maxRetries - 1)
            {
                return StatusCode(
                    StatusCodes.Status503ServiceUnavailable,
                    new ProblemDetails
                    {
                        Title = "No room code available",
                        Detail = "Could not generate a unique room code. Please try again."
                    }
                );
            }
        }

        var session = new Session
        {
            SurveyId = survey.Id,
            Name = data.Name,
            Description = data.Description,
            RoomCode = roomCode,
        };

        foreach (var template in survey.QuestionTemplates)
        {
            var question = new Question
            {
                SessionId = session.Id,
                QuestionTemplateId = template.Id,
            };
            session.Questions.Add(question);
        }

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return Ok(_mapper.MapToCreateSessionResponseDto(session));
    }

    [HttpGet("checkSession")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckSession([FromQuery] string roomCode)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(x => x.RoomCode == roomCode.Trim());
        if (session == null || session.RoomActive == false) return NotFound(new ProblemDetails
        {
            Title = "Room not available",
            Detail = $"No active room with code {roomCode} could be found"
        });

        return Ok();
    }

    [HttpGet("session")]
    [ProducesResponseType(typeof(SessionResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Authorize]
    public async Task<IActionResult> GetSession([FromQuery] Guid sessionId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var session = await _context.Sessions
            .Include(x => x.Survey)
            .Include(x => x.Questions.OrderBy(y => y.QuestionTemplate!.OrderNumber))
                .ThenInclude(x => x.Answers)
            .Include(x => x.Questions)
                .ThenInclude(x => x.QuestionTemplate)
            .Include(x => x.Questions)
                .ThenInclude(x => (x.QuestionTemplate as ChoiceQuestionTemplate)!.AnswerOptions.OrderBy(y => y.OrderNumber))
            .FirstOrDefaultAsync(x => x.Id == sessionId && x.Survey!.OwnerId == user.Id);
        if (session == null) return NotFound();
        if (session.RoomActive == true) return Conflict();

        var result = new SessionResultDto
        {
            Name = session.Name,
            Description = session.Description,
            OpenedAt = session.OpenedAt,
        };

        foreach (var question in session.Questions)
        {
            var aggregate = await _answerService.GetAggregateResultsAsync(question.Id, question.QuestionTemplate!.QuestionType);
            if (aggregate == null) continue;

            result.Questions.Add(new QuestionDto
            {
                QuestionTemplateDto = _mapper.MapToQuestionTemplateDto(question.QuestionTemplate!),
                AnswerDisplayDto = aggregate
            });
        }

        return Ok(result);
    }

    [HttpGet("getSessionList")]
    [Authorize]
    [ProducesResponseType(typeof(PaginatedSessionListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSessionList([FromQuery] int pageSize = 10, [FromQuery] int currentPage = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var sessionQuery = _context.Sessions
            .AsNoTracking()
            .Include(x => x.AnonymousParticipants)
            .Where(x => x.Survey!.OwnerId == user.Id && x.RoomActive == false);

        var sessionCount = await sessionQuery.CountAsync();

        var sessionListInfo = await sessionQuery
            .OrderByDescending(x => x.OpenedAt)
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(x => _mapper.MapToSessionListInfoDto(x))
            .ToListAsync();

        var result = new PaginatedSessionListDto
        {
            SessionCount = sessionCount,
            sessionListInfo = sessionListInfo
        };

        return Ok(result);
    }

    [HttpGet("openSessions")]
    [ProducesResponseType(typeof(IEnumerable<GetOpenSessionsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOpenSessions()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var sessions = await _context.Sessions
            .AsNoTracking()
            .Where(x => x.Survey!.OwnerId == user.Id && x.RoomActive == true)
            .OrderBy(x => x.OpenedAt)
            .Select(x => _mapper.MapToGetOpenSessionsDto(x))
            .ToListAsync();

        return Ok(sessions);
    }

    [HttpPatch("closeSessionBatch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CloseSessionBatch([FromBody] List<Guid> sessionIds)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var sessions = await _context.Sessions
            .Where(x =>
                x.Survey!.OwnerId == user.Id &&
                x.RoomActive == true
            )
            .ToListAsync();

        var sessionIdsHashSet = new HashSet<Guid>(sessionIds);
        var sessionsToClose = sessions
            .Where(x => sessionIdsHashSet.Contains(x.Id))
            .ToList();

        foreach (var session in sessionsToClose)
        {
            session.RoomActive = false;
            await _hubContext.Clients.Group(session.RoomCode).SessionClosed();
        }

        await _context.SaveChangesAsync();

        return Ok();
    }
}