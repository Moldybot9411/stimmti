using Backend.Dto;
using Backend.Models;

namespace Backend.Mapper;

public interface IApiMapper
{
    AnonymousUserDto MapToAnonymousUserDto(AnonymousUser source);
    CreateSessionResponseDto MapToCreateSessionResponseDto(Session source);
    PresenterDto MapToPresenterDto(User source);
    ParticipantDto MapToParticipantDto(AnonymousUser source);
    AnonymousProfilePictureDto MapToAnonymousProfilePictureDto(AnonymousProfilePicture source);
    void UpdateAnonymousProfilePicture(AnonymousProfilePictureDto source, AnonymousProfilePicture target);
    QuestionTemplateDto MapToQuestionTemplateDto(QuestionTemplate source);
    List<ParticipantDto> MapToParticipantDtoList(IEnumerable<AnonymousUser> source);
    SessionListInfoDto MapToSessionListInfoDto(Session source);
}
