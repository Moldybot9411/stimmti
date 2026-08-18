using Backend.Dto;
using Backend.Models.Enums;

namespace Backend.Services.Dto;

public class JoinSessionServiceDto
{
    public required RestoreStateDto RestoreStateDto { get; set; }
    public Guid? QuestionId { get; set; }
    public QuestionTypeEnum? QuestionType { get; set; }
}