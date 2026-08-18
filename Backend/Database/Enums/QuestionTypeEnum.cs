using Tapper;

namespace Backend.Models.Enums;

[TranspilationSource]
public enum QuestionTypeEnum
{
    SingleChoice = 1,
    MultipleChoice = 2,
    WordCloud = 3,
    FreeText = 4,
    NumberScale = 5
}