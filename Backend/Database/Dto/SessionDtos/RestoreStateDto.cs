using Backend.Models.Enums;
using Microsoft.Net.Http.Headers;
using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class RestoreStateDto
{
    public Guid? SessionId { get; set; } // Only sent to admin to redirect to statistics page of the session
    public required string SessionName { get; set; }
    public string? SessionDescription { get; set; }

    public required ParticipantRole Role { get; set; }
    public required SessionState SessionState { get; set; }
    public AnonymousUserDto? UserInformation { get; set; }
    public required PresenterDto Presenter { get; set; }
    public List<ParticipantDto> Participants { get; set; } = new List<ParticipantDto>();
    public QuestionTemplateDto? CurrentQuestion { get; set; }
    public bool AnsweredThisRound { get; set; } = false;
}