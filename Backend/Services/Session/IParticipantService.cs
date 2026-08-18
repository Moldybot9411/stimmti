using Backend.Dto;
using Backend.Services.Dto;

namespace Backend.Services;

public interface IParticipantService
{
    Task<JoinSessionServiceDto?> JoinSessionAsync(JoinSessionDto data, string connectionId, string? userIdentifier);
    Task<bool> UpdateParticipantDataAsync(ParticipantUpdateDto data);
}