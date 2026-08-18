using Backend.Models.Enums;
using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class QuestionTemplateDto
{
    public required Guid Id;
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required QuestionTypeEnum QuestionType { get; set; }
    public List<AnswerOptionDto> AnswerOptions { get; } = new List<AnswerOptionDto>();
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? WordCloudMaxWords { get; set; }
}