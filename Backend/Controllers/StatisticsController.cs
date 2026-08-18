using Backend.Dto;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly StimmtiDbContext _context;
    private readonly UserManager<User> _userManager;

    public StatisticsController(StimmtiDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("statistics")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(GetStatisticsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics([FromQuery] int pageSize = 10, [FromQuery] int currentSurveyPage = 1, [FromQuery] int currentSessionPage = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var oneMonthAgo = DateTimeOffset.Now.AddDays(-30);

        // --- Survey Statistics
        var surveysQuery = _context.Surveys
            .Where(x => x.OwnerId == user.Id);

        var surveyCount = await surveysQuery.CountAsync();
        var surveyDelta = await surveysQuery
            .Where(x => x.CreatedAt > oneMonthAgo)
            .CountAsync();

        var pagedSurveys = await surveysQuery
            .OrderByDescending(x => x.Sessions.Count())
            .ThenBy(x => x.Title)
            .Skip((currentSurveyPage - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SurveyStatisticsDto
            {
                Name = x.Title,
                NumSessions = x.Sessions.Count()
            })
            .ToListAsync();

        // --- Session Statistics
        var sessionsQuery = _context.Sessions
            .Where(x => x.Survey!.OwnerId == user.Id && x.RoomActive == false);

        var sessionCount = await sessionsQuery.CountAsync();
        var SessionDelta = await sessionsQuery
            .Where(x => x.OpenedAt > oneMonthAgo)
            .CountAsync();

        var participantCount = await sessionsQuery
            .SumAsync(x => x.AnonymousParticipants.Count());
        var participantDelta = await sessionsQuery
            .Where(x => x.OpenedAt > oneMonthAgo)
            .SumAsync(x => x.AnonymousParticipants.Count());

        var pagedSessions = await sessionsQuery
            .OrderByDescending(x => x.AnonymousParticipants.Count())
            .ThenByDescending(x => x.OpenedAt)
            .ThenBy(x => x.Name)
            .Skip((currentSessionPage - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SessionStatisticsDto
            {
                Name = x.Name,
                ParticipantCount = x.AnonymousParticipants.Count(),
                OpenedAt = x.OpenedAt
            })
            .ToListAsync();

        var result = new GetStatisticsDto
        {
            SurveyCount = surveyCount,
            SurveyDelta = surveyDelta,
            SessionCount = sessionCount,
            SessionDelta = SessionDelta,
            ParticipantCount = participantCount,
            ParticipantDelta = participantDelta,
        };

        result.SurveyStatistics.AddRange(pagedSurveys);
        result.SessionStatistics.AddRange(pagedSessions);

        return Ok(result);
    }
}