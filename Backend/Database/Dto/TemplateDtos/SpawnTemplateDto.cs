using System.ComponentModel.DataAnnotations;
using Backend.Models.Enums;

namespace Backend.Dto;

public class SpawnTemplateDto
{
    [MaxLength(255)]
    public required string Title { get; set; }

    [MaxLength(2048)]
    public string? Description { get; set; }

    public required TemplateType TemplateType { get; set; }
}