using Backend.Dto;
using Backend.Models;
using Backend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Backend.StaticHelpers.TemplateQuestions;

[ApiController]
[Route("/api/v1/[controller]")]
public class TemplateController : ControllerBase
{
    private readonly StimmtiDbContext _dbContext;
    private readonly UserManager<User> _userManager;

    public TemplateController(StimmtiDbContext dbContext, UserManager<User> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SpawnTemplate(SpawnTemplateDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var title = data.Title.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest(new ProblemDetails { Title = "Missing Title", Detail = "No title was provided" });
        }

        var survey = new Survey
        {
            Title = title,
            Description = data.Description,
            OwnerId = user.Id,
        };

        _dbContext.Surveys.Add(survey);

        switch (data.TemplateType)
        {
            case TemplateType.QuickPoll:
                AddQuickPollQuestions(survey.Id, _dbContext);
                break;
            case TemplateType.FeedbackForm:
                AddFeedbackFormQuestions(survey.Id, _dbContext);
                break;
            case TemplateType.TeamPulse:
                AddTeamPulseQuestions(survey.Id, _dbContext);
                break;

            default:
                AddQuickPollQuestions(survey.Id, _dbContext);
                break;
        }

        await _dbContext.SaveChangesAsync();

        return Ok(survey.Id);
    }
}