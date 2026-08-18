using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class Folder
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(255)]
    public required string Name { get; set; }

    public required Guid OwnerId { get; set; }
    public User? Owner { get; set; }

    public List<Survey> Surveys { get; } = new List<Survey>();
}