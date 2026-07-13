using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class SubmitAnswerDto
{
    public required Guid AnonymousUserId { get; set; }
    public required string RoomCode { get; set; }
    public List<AnswerOptionDto> AnswerOptions { get; set; } = new List<AnswerOptionDto>();
    public string? Text { get; set; }
    public int? Value { get; set; }
    public List<string> WordCloudAnswers { get; set; } = new List<string>();
}