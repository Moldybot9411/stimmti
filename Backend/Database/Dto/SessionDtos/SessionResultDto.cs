using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class SessionResultDto
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTimeOffset OpenedAt { get; set; }

    [Required]
    public List<QuestionDto> Questions { get; } = new List<QuestionDto>();
}