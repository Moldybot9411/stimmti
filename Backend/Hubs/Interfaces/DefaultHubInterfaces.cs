using Backend.Dto;
using Backend.Models.Enums;
using TypedSignalR.Client;

namespace Backend.Hubs.Interfaces;

[Hub]
public interface ISessionHub
{
    Task<RestoreStateDto?> JoinSession(JoinSessionDto data);
    Task LeaveRoom(string roomCode);
    Task<bool> UpdateParticipantData(ParticipantUpdateDto data);
    Task<bool> StartSession(string roomCode);
    Task<bool> NextQuestion(string roomCode);
    Task<bool> CloseSession(string roomCode);
    Task<bool> SubmitAnswer(SubmitAnswerDto data);
}

[Receiver]
public interface ISessionHubClient
{
    Task ParticipantJoined(ParticipantDto data);
    Task ParticipantUpdated(ParticipantUpdateResponseDto data);
    Task SessionStateChanged(SessionState newState);
    Task QuestionChanged(QuestionTemplateDto data);
    Task AnswerSubmitted(AnswerDisplayDto data);
    Task SessionClosed();
}