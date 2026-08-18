using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class AnonymousUserDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required AnonymousProfilePictureDto ProfilePicture { get; set; }
}