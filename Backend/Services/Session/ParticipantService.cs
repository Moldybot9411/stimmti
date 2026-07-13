using Backend.Dto;
using Backend.Hubs;
using Backend.Hubs.Interfaces;
using Backend.Mapper;
using Backend.Models;
using Backend.Models.Enums;
using Backend.Services.Dto;
using Backend.StaticHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services;

public class ParticipantService : IParticipantService
{
    private readonly StimmtiDbContext _context;
    private readonly IHubContext<DefaultHub, ISessionHubClient> _hubContext;
    private readonly IApiMapper _mapper;

    public ParticipantService(StimmtiDbContext context, IHubContext<DefaultHub, ISessionHubClient> hubContext, IApiMapper mapper)
    {
        _context = context;
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task<JoinSessionServiceDto?> JoinSessionAsync(JoinSessionDto data, string connectionId, string? userIdentifier)
    {
        var session = await _context.Sessions
            .Include(x => x.AnonymousParticipants)
                .ThenInclude(x => x.ProfilePicture)
            .Include(x => x.Survey)
                .ThenInclude(x => x!.Owner)
            .Include(x => x.CurrentQuestion)
                .ThenInclude(x => (x!.QuestionTemplate as ChoiceQuestionTemplate)!.AnswerOptions)
            .Include(x => x.CurrentQuestion)
                .ThenInclude(x => x.Answers)
            .FirstOrDefaultAsync(x => x.RoomCode == data.RoomCode && x.RoomActive == true);

        if (session == null) return null;

        bool isPresenter = userIdentifier != null && session.Survey!.OwnerId.ToString() == userIdentifier;

        if (isPresenter)
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, data.RoomCode);

            return new JoinSessionServiceDto
            {
                RestoreStateDto = new RestoreStateDto
                {
                    SessionId = session.Id,
                    SessionName = session.Name,
                    SessionDescription = session.Description,
                    Role = ParticipantRole.Presenter,
                    SessionState = session.CurrentState,
                    Presenter = _mapper.MapToPresenterDto(session.Survey!.Owner!),
                    Participants = _mapper.MapToParticipantDtoList(session.AnonymousParticipants),
                    CurrentQuestion = session.CurrentQuestion?.QuestionTemplate != null
                    ? _mapper.MapToQuestionTemplateDto(session.CurrentQuestion.QuestionTemplate)
                    : null
                },
                QuestionId = session.CurrentQuestionId,
                QuestionType = session.CurrentQuestion?.QuestionTemplate?.QuestionType
            };
        }

        // --- Anonymous participant
        var participant = session.AnonymousParticipants.FirstOrDefault(x => x.Id == data.playerId);
        if (participant == null)
        {
            participant = new AnonymousUser
            {
                Name = NameGenerator.GenerateName("", " "),
                SessionId = session.Id,
            };

            await _context.AddAsync(participant);

            var rng = new Random();
            T RandomEnum<T>() where T : struct, Enum
            {
                var values = Enum.GetValues<T>();
                return values[rng.Next(values.Length)];
            }

            var anonProfilePicture = new AnonymousProfilePicture
            {
                Body = RandomEnum<BodyProfileEnum>(),
                Color = RandomEnum<ColorProfileEnum>(),
                Face = RandomEnum<FaceProfileEnum>(),
                Hat = RandomEnum<HatProfileEnum>(),
                AnonymousUserId = participant.Id
            };

            await _context.AddAsync(anonProfilePicture);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(data.RoomCode).ParticipantJoined(new ParticipantDto
            {
                Name = participant.Name,
                ProfilePicture = _mapper.MapToAnonymousProfilePictureDto(anonProfilePicture)
            });
        }

        await _hubContext.Groups.AddToGroupAsync(connectionId, data.RoomCode);

        return new JoinSessionServiceDto
        {
            RestoreStateDto = new RestoreStateDto
            {
                SessionName = session.Name,
                SessionDescription = session.Description,
                Role = ParticipantRole.Participant,
                SessionState = session.CurrentState,
                UserInformation = _mapper.MapToAnonymousUserDto(participant),
                Presenter = _mapper.MapToPresenterDto(session.Survey!.Owner!),
                Participants = _mapper.MapToParticipantDtoList(session.AnonymousParticipants),
                CurrentQuestion = session.CurrentQuestion?.QuestionTemplate != null
                ? _mapper.MapToQuestionTemplateDto(session.CurrentQuestion.QuestionTemplate)
                : null,
                AnsweredThisRound = participant.AnsweredThisRound
            },
            QuestionId = session.CurrentQuestionId,
            QuestionType = session.CurrentQuestion?.QuestionTemplate?.QuestionType
        };
    }

    public async Task<bool> UpdateParticipantDataAsync(ParticipantUpdateDto data)
    {
        var participant = await _context.AnonymousUsers
            .Include(x => x.ProfilePicture)
            .Include(x => x.Session)
            .FirstOrDefaultAsync(x => x.Id == data.AnonymousUserId);

        if (participant == null) return false;
        if (participant.Session!.RoomCode != data.RoomCode) return false;
        if (participant.Session!.RoomActive == false) return false;
        if (!string.IsNullOrWhiteSpace(data.Name) && data.Name.Length > 64) return false;

        var oldName = participant.Name;

        if (!string.IsNullOrWhiteSpace(data.Name))
        {
            var nameAlreadyExists = await _context.AnonymousUsers
                .AnyAsync(p =>
                    p.Session!.RoomCode == data.RoomCode &&
                    p.Name.ToLower() == data.Name.Trim().ToLower() &&
                    p.Id != data.AnonymousUserId
                );

            if (nameAlreadyExists) return false;

            participant.Name = data.Name.Trim();
        }

        if (data.ProfilePicture != null)
        {
            _mapper.UpdateAnonymousProfilePicture(data.ProfilePicture, participant.ProfilePicture!);
        }

        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group(data.RoomCode).ParticipantUpdated(new ParticipantUpdateResponseDto
        {
            OldName = oldName,
            NewName = participant.Name,
            ProfilePicture = _mapper.MapToAnonymousProfilePictureDto(participant.ProfilePicture!)
        });

        return true;
    }
}