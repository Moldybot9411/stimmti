using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Survey
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(255)]
    public required string Title { get; set; }

    [MaxLength(2048)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? FolderId { get; set; }
    public Folder? Folder { get; set; }

    public required Guid OwnerId { get; set; }
    public User? Owner { get; set; }

    public List<Session> Sessions { get; } = new List<Session>();
    public List<QuestionTemplate> QuestionTemplates { get; } = new List<QuestionTemplate>();
}