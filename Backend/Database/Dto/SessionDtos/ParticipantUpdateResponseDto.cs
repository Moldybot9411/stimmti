using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class ParticipantUpdateResponseDto
{
    public required string OldName { get; set; }
    public required string NewName { get; set; }
    public required AnonymousProfilePictureDto ProfilePicture { get; set; }
}