using Backend.Models.Enums;
using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class AnonymousProfilePictureDto
{
    public required BodyProfileEnum Body { get; set; }
    public required ColorProfileEnum Color { get; set; }
    public required FaceProfileEnum Face { get; set; }
    public required HatProfileEnum Hat { get; set; }
}