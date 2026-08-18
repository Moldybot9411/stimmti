namespace Backend.Models.Enums;

public enum JoinSessionError
{
    Unknown = 0,
    SessionNotFound = 1,
    SessionInactive = 2,
    NicknameRequired = 3,
    NicknameTaken = 4
}
