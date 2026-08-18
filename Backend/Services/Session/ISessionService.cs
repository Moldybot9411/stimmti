namespace Backend.Services;

public interface ISessionService
{
    Task<bool> StartSessionAsync(string roomCode, string? userIdentifier);
    Task<bool> NextQuestionAsync(string roomCode, string? userIdentifier);
    Task<bool> CloseSessionAsync(string roomCode, string? userIdentifier);
}