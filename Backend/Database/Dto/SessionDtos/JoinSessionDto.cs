using Tapper;

namespace Backend.Dto;

[TranspilationSource]
public class JoinSessionDto
{
    public required string RoomCode { get; set; }
    public Guid? playerId { get; set; }
}