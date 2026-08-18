using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class ParticipantDto
{
    public required string Name { get; set; }
    public required AnonymousProfilePictureDto ProfilePicture { get; set; }
}