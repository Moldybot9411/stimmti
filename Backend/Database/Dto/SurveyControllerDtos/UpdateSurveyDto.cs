using System.ComponentModel.DataAnnotations;

public class UpdateSurveyDto
{
    [MaxLength(255)]
    public string? Title { get; set; }

    [MaxLength(2048)]
    public string? Description { get; set; }

    public Guid? FolderId { get; set; }

    public bool? RemoveFromFolder { get; set; }
}