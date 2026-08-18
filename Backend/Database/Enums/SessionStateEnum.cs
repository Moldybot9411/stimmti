using Tapper;

namespace Backend.Models.Enums;

[TranspilationSource]
public enum SessionState
{
    Lobby = 1,
    Loading,
    Question,
    Finished,
}