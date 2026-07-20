using Backend.Dto;
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

	public SurveyController(StimmtiDbContext dbContext, UserManager<User> userManager)
	{
		_dbContext = dbContext;
		_userManager = userManager;
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
	[ProducesResponseType(typeof(IEnumerable<GetSurveyResponseDto>), StatusCodes.Status200OK)]
	public async Task<IActionResult> GetSurveys()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null) return Unauthorized();

		var surveys = await _dbContext.Surveys
			.AsNoTracking()
			.Where(x => x.OwnerId == user.Id && x.FolderId == null)
			.OrderBy(x => x.Title)
			.Select(x => new GetSurveyResponseDto
			{
				SurveyId = x.Id,
				Title = x.Title,
				Description = x.Description,
				FolderId = x.FolderId,
			})
			.ToListAsync();

		return Ok(surveys);
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
		else if (data.FolderId.HasValue)
		{
			var folder = await _dbContext.Folders
				.FirstOrDefaultAsync(x => x.Id == data.FolderId && x.OwnerId == user.Id);

			if (folder == null)
			{
				return NotFound(new ProblemDetails { Title = "Folder not found", Detail = $"Folder with ID {data.FolderId} couldn't be found" });
			}

			survey.FolderId = data.FolderId.Value;
		}
		else return BadRequest(new ProblemDetails
		{
			Title = "Invalid request",
			Detail = "You must either provide a FolderId or set RemoveFromFolder to true"
		});

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
	[ProducesResponseType(typeof(IEnumerable<GetFolderResponseDto>), StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<GetFolderResponseDto>>> GetFolders()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null) return Unauthorized();

		var folders = await _dbContext.Folders
			.AsNoTracking()
			.Where(x => x.OwnerId == user.Id)
			.OrderBy(x => x.Name)
			.Select(x => new GetFolderResponseDto
			{
				FolderId = x.Id,
				Name = x.Name,
				Surveys = x.Surveys
					.OrderBy(s => s.Title)
					.Select(s => new GetSurveyResponseDto
					{
						SurveyId = s.Id,
						Title = s.Title,
						Description = s.Description,
						FolderId = s.FolderId,
					})
					.ToList()
			})
			.ToListAsync();

		return Ok(folders);
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
}