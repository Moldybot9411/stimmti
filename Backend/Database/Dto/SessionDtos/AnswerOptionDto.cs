using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class AnswerOptionDto
{
    public required Guid Id { get; set; }
    public required string Description { get; set; }
}