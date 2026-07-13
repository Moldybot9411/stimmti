using Backend.Dto;
using Backend.Models;
using Riok.Mapperly.Abstractions;

// RMG020: Intentional partial mappings – source models contain more fields than the DTOs
#pragma warning disable RMG020

namespace Backend.Mapper;

[Mapper]
public partial class ApiMapper : IApiMapper
{
    public partial AnonymousUserDto MapToAnonymousUserDto(AnonymousUser source);
    public partial CreateSessionResponseDto MapToCreateSessionResponseDto(Session source);
    public partial PresenterDto MapToPresenterDto(User source);
    public partial ParticipantDto MapToParticipantDto(AnonymousUser source);
    public partial AnonymousProfilePictureDto MapToAnonymousProfilePictureDto(AnonymousProfilePicture source);

    [MapperIgnoreTarget(nameof(AnonymousProfilePicture.Id))]
    [MapperIgnoreTarget(nameof(AnonymousProfilePicture.AnonymousUserId))]
    [MapperIgnoreTarget(nameof(AnonymousProfilePicture.AnonymousUser))]
    public partial void UpdateAnonymousProfilePicture(AnonymousProfilePictureDto source, AnonymousProfilePicture target);

    public QuestionTemplateDto MapToQuestionTemplateDto(QuestionTemplate source)
    {
        var dto = new QuestionTemplateDto
        {
            Id = source.Id,
            Name = source.Name,
            Description = source.Description,
            QuestionType = source.QuestionType,
        };

        if (source is ChoiceQuestionTemplate choice)
        {
            dto.AnswerOptions.AddRange(
                choice.AnswerOptions
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => new AnswerOptionDto { Id = x.Id, Description = x.Description })
            );
        }

        if (source is NumberScaleQuestionTemplate numberScale)
        {
            dto.MinValue = numberScale.MinValue;
            dto.MaxValue = numberScale.MaxValue;
        }

        if (source is WordCloudQuestionTemplate wordCloud)
        {
            dto.WordCloudMaxWords = wordCloud.MaxWords;
        }

        return dto;
    }

    public partial List<ParticipantDto> MapToParticipantDtoList(IEnumerable<AnonymousUser> source);
}