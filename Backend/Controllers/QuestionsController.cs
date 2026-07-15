
using Backend.Models;
using Backend.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly StimmtiDbContext _context;
    private readonly UserManager<User> _userManager;

    public QuestionsController(
        StimmtiDbContext context,
        UserManager<User> userManager
    )
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet("{questionId}")]
    [Authorize]
    [ProducesResponseType(typeof(GetQuestionTemplateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetQuestionTemplate(Guid questionId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var questionTemplate = await _context.QuestionTemplates
            .Include(x => x.Survey)
            .Include(x => (x as ChoiceQuestionTemplate)!.AnswerOptions)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (questionTemplate == null)
            return BadRequest(new ProblemDetails { Title = "Question Template not found", Detail = $"Question Template with ID {questionId} doesn't exist" });

        if (questionTemplate.Survey == null)
            return BadRequest(new ProblemDetails { Title = "Survey not found", Detail = "Question template has no linked survey" });

        if (questionTemplate.Survey.OwnerId != user.Id)
            return Forbid();

        var responseDto = new GetQuestionTemplateResponseDto
        {
            Id = questionTemplate.Id,
            Name = questionTemplate.Name,
            Description = questionTemplate.Description,
            SurveyId = questionTemplate.SurveyId,
            OrderNumber = questionTemplate.OrderNumber,
            IsArchived = questionTemplate.IsArchived,
            QuestionType = questionTemplate.QuestionType,
            MinValue = (questionTemplate as NumberScaleQuestionTemplate)?.MinValue ?? 0,
            MaxValue = (questionTemplate as NumberScaleQuestionTemplate)?.MaxValue ?? 0,
            MaxWords = (questionTemplate as WordCloudQuestionTemplate)?.MaxWords ?? 0,
            Answers = (questionTemplate as ChoiceQuestionTemplate)?.AnswerOptions
                .OrderBy(x => x.OrderNumber)
                .Select(x => x.Description)
                .ToArray() ?? Array.Empty<string>()
        };

        return Ok(responseDto);
    }

    [HttpPost]
	[Authorize]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PostQuestionTemplate([FromBody] CreateQuestionTemplateDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var survey = await _context.Surveys
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == data.SurveyId);

        if (survey == null)
            return BadRequest(new ProblemDetails { Title = "Survey not found", Detail = $"Survey with ID {data.SurveyId} doesn't exist" });

        if (survey.OwnerId != user.Id)
            return Forbid();

        var name = data.Name.Trim();
        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new ProblemDetails { Title = "Invalid Name", Detail = "Question template name must not be empty" });

        if (data.QuestionType == QuestionTypeEnum.NumberScale && data.MinValue >= data.MaxValue)
            return BadRequest(new ProblemDetails { Title = "Invalid Number Scale", Detail = "MinValue must be smaller than MaxValue" });

        if (data.QuestionType == QuestionTypeEnum.WordCloud && data.MaxWords <= 0)
            return BadRequest(new ProblemDetails { Title = "Invalid Word Cloud", Detail = "MaxWords must be greater than 0" });

        var cleanedAnswers = (data.Answers ?? Array.Empty<string>())
            .Select(x => x?.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if ((data.QuestionType == QuestionTypeEnum.SingleChoice || data.QuestionType == QuestionTypeEnum.MultipleChoice) && cleanedAnswers.Count < 2)
            return BadRequest(new ProblemDetails { Title = "Invalid Answers", Detail = "SingleChoice and MultipleChoice require at least 2 answer options" });

        if (data.QuestionType != QuestionTypeEnum.SingleChoice && data.QuestionType != QuestionTypeEnum.MultipleChoice && cleanedAnswers.Count > 0)
            return BadRequest(new ProblemDetails { Title = "Invalid Question Type", Detail = "Answer options can only be provided for SingleChoice and MultipleChoice" });

        QuestionTemplate questionTemplate = data.QuestionType switch
        {
            QuestionTypeEnum.SingleChoice => new SingleChoiceQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = data.Description?.Trim(),
                SurveyId = data.SurveyId,
                OrderNumber = data.OrderNumber,
                IsArchived = data.IsArchived,
            },
            QuestionTypeEnum.MultipleChoice => new MultipleChoiceQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = data.Description?.Trim(),
                SurveyId = data.SurveyId,
                OrderNumber = data.OrderNumber,
                IsArchived = data.IsArchived,
            },
            QuestionTypeEnum.WordCloud => new WordCloudQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = data.Description?.Trim(),
                SurveyId = data.SurveyId,
                OrderNumber = data.OrderNumber,
                IsArchived = data.IsArchived,
                MaxWords = data.MaxWords,
            },
            QuestionTypeEnum.FreeText => new FreeTextQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = data.Description?.Trim(),
                SurveyId = data.SurveyId,
                OrderNumber = data.OrderNumber,
                IsArchived = data.IsArchived,
            },
            QuestionTypeEnum.NumberScale => new NumberScaleQuestionTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = data.Description?.Trim(),
                SurveyId = data.SurveyId,
                OrderNumber = data.OrderNumber,
                IsArchived = data.IsArchived,
                MinValue = data.MinValue,
                MaxValue = data.MaxValue,
            },
            _ => throw new InvalidOperationException($"Unsupported QuestionType: {data.QuestionType}"),
        };

        if (questionTemplate is ChoiceQuestionTemplate choiceTemplate)
        {
            choiceTemplate.AnswerOptions.AddRange(
                cleanedAnswers.Select((answer, index) => new AnswerOption
                {
                    OrderNumber = index + 1,
                    Description = answer!,
                    QuestionTemplateId = choiceTemplate.Id,
                })
            );
        }

        _context.QuestionTemplates.Add(questionTemplate);
        await _context.SaveChangesAsync();

        return Ok(questionTemplate.Id);
    }

    [HttpPatch("{questionId:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PatchQuestionTemplate(Guid questionId, [FromBody] UpdateQuestionTemplateResponseDto data)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var questionTemplate = await _context.QuestionTemplates
            .Include(x => x.Survey)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (questionTemplate == null)
            return BadRequest(new ProblemDetails { Title = "Question Template not found", Detail = $"Question Template with ID {questionId} doesn't exist" });

        if (questionTemplate.Survey == null)
            return BadRequest(new ProblemDetails { Title = "Survey not found", Detail = "Question template has no linked survey" });

        if (questionTemplate.Survey.OwnerId != user.Id)
            return Forbid();

        if (data.Name != null)
        {
            var trimmedName = data.Name.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
                return BadRequest(new ProblemDetails { Title = "Invalid Name", Detail = "Question template name must not be empty" });

            questionTemplate.Name = trimmedName;
        }

        if (data.Description != null)
            questionTemplate.Description = data.Description.Trim();

        if (data.OrderNumber.HasValue)
            questionTemplate.OrderNumber = data.OrderNumber.Value;

        if (data.IsArchived.HasValue)
            questionTemplate.IsArchived = data.IsArchived.Value;

        if (questionTemplate is NumberScaleQuestionTemplate numberScaleTemplate)
        {
            var minValue = data.MinValue ?? numberScaleTemplate.MinValue;
            var maxValue = data.MaxValue ?? numberScaleTemplate.MaxValue;

            if (minValue >= maxValue)
                return BadRequest(new ProblemDetails { Title = "Invalid Number Scale", Detail = "MinValue must be smaller than MaxValue" });

            numberScaleTemplate.MinValue = minValue;
            numberScaleTemplate.MaxValue = maxValue;
        }
        else if (data.MinValue.HasValue || data.MaxValue.HasValue)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid Question Type", Detail = "MinValue and MaxValue can only be set for NumberScale questions" });
        }

        if (questionTemplate is WordCloudQuestionTemplate wordCloudTemplate)
        {
            if (data.MaxWords.HasValue)
            {
                if (data.MaxWords.Value <= 0)
                    return BadRequest(new ProblemDetails { Title = "Invalid Word Cloud", Detail = "MaxWords must be greater than 0" });

                wordCloudTemplate.MaxWords = data.MaxWords.Value;
            }
        }
        else if (data.MaxWords.HasValue)
        {
            return BadRequest(new ProblemDetails { Title = "Invalid Question Type", Detail = "MaxWords can only be set for WordCloud questions" });
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{questionId:guid}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteQuestionTemplate(Guid questionId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var questionTemplate = await _context.QuestionTemplates
            .Include(x => x.Survey)
            .FirstOrDefaultAsync(x => x.Id == questionId);

        if (questionTemplate == null)
            return BadRequest(new ProblemDetails { Title = "Question Template not found", Detail = $"Question Template with ID {questionId} doesn't exist" });

        if (questionTemplate.Survey == null)
            return BadRequest(new ProblemDetails { Title = "Survey not found", Detail = "Question template has no linked survey" });

        if (questionTemplate.Survey.OwnerId != user.Id)
            return Forbid();

        questionTemplate.IsArchived = true;
        _context.QuestionTemplates.Update(questionTemplate);
        await _context.SaveChangesAsync();

        return Ok();
    }
}

