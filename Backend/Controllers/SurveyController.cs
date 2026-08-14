using Backend.Dto;
using Backend.Mapper;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/[controller]")]
public class SurveyController : ControllerBase
{
    private readonly StimmtiDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly IApiMapper _mapper;

    public SurveyController(StimmtiDbContext dbContext, UserManager<User> userManager, IApiMapper mapper)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreateSurveyResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSurvey([FromBody] CreateSurveyDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();


        var title = data.Title.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            return BadRequest(new ProblemDetails { Title = "Missing Title", Detail = "No title was provided" });
        }

        if (data.FolderId.HasValue)
        {
            var folder = await _dbContext.Folders.FirstOrDefaultAsync(x => x.Id == data.FolderId.Value);
            if (folder == null)
            {
                return NotFound(new ProblemDetails { Title = "Folder not found", Detail = $"Folder with ID {data.FolderId} doesn't exist" });
            }

            if (folder.OwnerId != user.Id)
            {
                return Forbid();
            }
        }

        var survey = new Survey
        {
            Title = title,
            Description = data.Description,
            FolderId = data.FolderId,
            OwnerId = user.Id,
        };

        _dbContext.Surveys.Add(survey);
        await _dbContext.SaveChangesAsync();

        return Ok(new CreateSurveyResponseDto
        {
            SurveyId = survey.Id,
            FolderId = data.FolderId
        });
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(PaginatedSurveyListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSurveys([FromQuery] int pageSize = 10, [FromQuery] int currentPage = 1, [FromQuery] int skip = 0)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var query = _dbContext.Surveys
            .AsNoTracking()
            .Where(x => x.OwnerId == user.Id && x.FolderId == null && !x.IsArchived);

        var surveyCount = await query.CountAsync();

        var surveyListInfo = await query
            .OrderByDescending(x => x.IsFavorite)
            .ThenBy(x => x.Title)
            .Skip((currentPage - 1) * pageSize + skip)
            .Take(pageSize)
            .Select(x => new GetSurveyResponseDto
            {
                SurveyId = x.Id,
                Title = x.Title,
                Description = x.Description,
                FolderId = x.FolderId,
                IsFavorite = x.IsFavorite,
                QuestionAmount = x.QuestionTemplates.Count(q => !q.IsArchived)
            })
            .ToListAsync();

        var response = new PaginatedSurveyListDto
        {
            SurveyCount = surveyCount,
            SurveyListInfo = surveyListInfo
        };

        return Ok(response);
    }

    [HttpPatch("{surveyId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateSurvey(Guid surveyId, [FromBody] UpdateSurveyDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var survey = await _dbContext.Surveys.FirstOrDefaultAsync(x => x.Id == surveyId);
        if (survey == null) return NotFound();

        if (survey.OwnerId != user.Id) return Forbid();

        if (!string.IsNullOrWhiteSpace(data.Title)) survey.Title = data.Title.Trim();
        if (!string.IsNullOrWhiteSpace(data.Description)) survey.Description = data.Description.Trim();

        if (data.RemoveFromFolder.HasValue && data.RemoveFromFolder.Value) survey.FolderId = null;
        else if (data.FolderId.HasValue) survey.FolderId = data.FolderId.Value;
        else return BadRequest(new ProblemDetails { Title = "Invalid request", Detail = "You must either provide a FolderId or set RemoveFromFolder to true" });

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }


    [HttpPost("folders")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(CreateFolderResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var name = data.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new ProblemDetails { Title = "No name provided", Detail = "No folder name was provided" });
        }

        var folder = new Folder
        {
            Name = name,
            OwnerId = user.Id
        };

        _dbContext.Folders.Add(folder);
        await _dbContext.SaveChangesAsync();

        return Ok(new CreateFolderResponseDto
        {
            FolderId = folder.Id
        });
    }

    [HttpGet("folders")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(PaginatedFolderListDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFolders([FromQuery] int pageSize = 10, [FromQuery] int currentPage = 1)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var query = _dbContext.Folders
            .AsNoTracking()
            .Where(x => x.OwnerId == user.Id);

        var folderCount = await query.CountAsync();

        var folderListInfo = await query
            .OrderBy(x => x.Name)
            .Skip((currentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new GetFolderResponseDto
            {
                FolderId = x.Id,
                Name = x.Name,
                Surveys = x.Surveys
                    .Where(s => s.IsArchived == false)
                    .OrderByDescending(s => s.IsFavorite)
                    .ThenBy(s => s.Title)
                    .Select(s => new GetSurveyResponseDto
                    {
                        SurveyId = s.Id,
                        Title = s.Title,
                        Description = s.Description,
                        FolderId = s.FolderId,
                        IsFavorite = s.IsFavorite,
                        QuestionAmount = s.QuestionTemplates.Count(q => !q.IsArchived)
                    })
                    .ToList()
            })
            .ToListAsync();

        var response = new PaginatedFolderListDto
        {
            FolderCount = folderCount,
            folderListInfo = folderListInfo
        };

        return Ok(response);
    }

    [HttpPatch("folders/{folderId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateFolder(Guid folderId, [FromBody] UpdateFolderDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var folder = await _dbContext.Folders.FirstOrDefaultAsync(x => x.Id == folderId);
        if (folder == null) return NotFound();

        if (folder.OwnerId != user.Id)
        {
            return Forbid();
        }

        folder.Name = data.Name?.Trim() ?? folder.Name;

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("{surveyId:guid}/questions")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(IEnumerable<GetQuestionTemplateResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetQuestions(Guid surveyId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var survey = await _dbContext.Surveys.FirstOrDefaultAsync(x => x.Id == surveyId);
        if (survey == null) return NotFound();

        if (survey.OwnerId != user.Id)
        {
            return Forbid();
        }

        var questionTemplates = await _dbContext.QuestionTemplates
            .AsNoTracking()
            .Include(x => (x as ChoiceQuestionTemplate)!.AnswerOptions)
            .Where(x => x.SurveyId == surveyId && x.IsArchived == false)
            .OrderBy(x => x.OrderNumber)
            .ToListAsync();

        var questions = questionTemplates
            .Select(_mapper.MapToGetQuestionTemplateResponseDto)
            .ToList();

        return Ok(questions);
    }


    [HttpDelete("{surveyId:guid}")]
    [HttpDelete("/surveys/{surveyId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSurvey(Guid surveyId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var survey = await _dbContext.Surveys.FirstOrDefaultAsync(x => x.Id == surveyId);
        if (survey == null) return NotFound();

        if (survey.OwnerId != user.Id)
        {
            return Forbid();
        }


        var hasLinkedSession = await _dbContext.Questions
            .AnyAsync(x => x.Session != null);

        if (hasLinkedSession)
        {
            var questionTemplates = await _dbContext.QuestionTemplates
                .Where(x => x.SurveyId == surveyId && x.IsArchived == false)
                .ToListAsync();

            survey.IsArchived = true;

            foreach (var questionTemplate in questionTemplates)
            {
                questionTemplate.IsArchived = true;
            }
        }
        else
        {
            _dbContext.Surveys.Remove(survey);
            var questionTemplates = await _dbContext.QuestionTemplates
                .Where(x => x.SurveyId == surveyId)
                .ToListAsync();
            _dbContext.QuestionTemplates.RemoveRange(questionTemplates);
        }

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{surveyId:guid}/toggle-favorite")]
    [HttpPost("/surveys/{surveyId:guid}/toggle-favorite")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleFavoriteSurvey(Guid surveyId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var survey = await _dbContext.Surveys.FirstOrDefaultAsync(x => x.Id == surveyId);
        if (survey == null) return NotFound();

        if (survey.OwnerId != user.Id)
        {
            return Forbid();
        }

        survey.IsFavorite = !survey.IsFavorite;

        await _dbContext.SaveChangesAsync();

        return Ok(new { IsFavorite = survey.IsFavorite });
    }
}