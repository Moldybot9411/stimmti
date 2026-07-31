public class UpdateQuestionTemplateResponseDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? MinValue { get; set; }
    public int? MaxValue { get; set; }
    public int? MaxWords { get; set; }
    public List<string> Answers { get; set; } = new();
}