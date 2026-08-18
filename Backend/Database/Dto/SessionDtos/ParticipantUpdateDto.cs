using System.ComponentModel.DataAnnotations;
using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class ParticipantUpdateDto
{
    public required Guid AnonymousUserId { get; set; }
    public string? Name { get; set; }
    public AnonymousProfilePictureDto? ProfilePicture { get; set; }
    public required string RoomCode { get; set; }
}