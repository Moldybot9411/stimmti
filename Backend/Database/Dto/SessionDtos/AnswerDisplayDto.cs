using System.ComponentModel.DataAnnotations;
using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class AnswerDisplayDto
{
    [Required]
    public List<ChoiceResultDto> ChoiceResults { get; set; } = new();
    [Required]
    public List<FreeTextResultDto> FreeTextResults { get; set; } = new();
    [Required]
    public List<WordCloudResultDto> WordCloudResults { get; set; } = new();
    [Required]
    public List<NumberResultDto> NumberResults { get; set; } = new();

    public int TotalParticipantsAnswered { get; set; }
}

[TranspilationSource]
public class ChoiceResultDto
{
    public required AnswerOptionDto AnswerOption { get; set; }
    public required int Count { get; set; }
}

[TranspilationSource]
public class FreeTextResultDto
{
    public required Guid Id { get; set; }
    public required string Text { get; set; } = string.Empty;
}

[TranspilationSource]
public class WordCloudResultDto
{
    [Required]
    public required string Text { get; set; } = string.Empty;
    public required int Count { get; set; }
}

[TranspilationSource]
public class NumberResultDto
{
    public required int Value { get; set; }
    public required int Count { get; set; }
}