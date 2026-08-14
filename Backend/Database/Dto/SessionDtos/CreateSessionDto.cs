using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class CreateSessionDto
{
    [MaxLength(255)]
    public required string Name { get; set; }

    [MaxLength(2048)]
    public string? Description { get; set; }
    public required Guid SurveyId { get; set; }
}