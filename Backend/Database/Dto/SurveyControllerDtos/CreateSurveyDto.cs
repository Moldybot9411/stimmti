using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class CreateSurveyDto
{
    [MaxLength(255)]
    public required string Title { get; set; }

    [MaxLength(2048)]
    public string? Description { get; set; }

    public Guid? FolderId { get; set; }
}
