using System.ComponentModel.DataAnnotations;
using Backend.Models.Enums;

namespace Backend.Models;

public class Session
{
    [Key]
    public Guid Id { get; set; }

    [MaxLength(255)]
    public required string Name { get; set; }

    [MaxLength(2048)]
    public string? Description { get; set; }

    [MaxLength(10)]
    public required string RoomCode { get; set; }

    public bool RoomActive { get; set; } = true;

    public SessionState CurrentState { get; set; } = SessionState.Lobby;

    public DateTimeOffset OpenedAt { get; set; } = DateTimeOffset.UtcNow;

    public required Guid SurveyId { get; set; }
    public Survey? Survey { get; set; }

    public Guid? CurrentQuestionId { get; set; }
    public Question? CurrentQuestion { get; set; }

    public List<AnonymousUser> AnonymousParticipants { get; } = new List<AnonymousUser>();
    public List<Question> Questions { get; } = new List<Question>();
}