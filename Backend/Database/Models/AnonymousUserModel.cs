using System.ComponentModel.DataAnnotations;

namespace Backend.Models;

public class AnonymousUser
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(64)]
    public required string Name { get; set; }

    public required Guid SessionId { get; set; }
    public Session? Session { get; set; }

    public bool AnsweredThisRound { get; set; } = false;

    public AnonymousProfilePicture? ProfilePicture { get; set; }

    public List<Answer> Answers { get; set; } = new List<Answer>();
}