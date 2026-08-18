using System.ComponentModel.DataAnnotations;

namespace Backend.Dto;

public class CreateFolderDto
{
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
}
