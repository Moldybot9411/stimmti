using System.ComponentModel.DataAnnotations;

public class UpdateFolderDto
{
    [MaxLength(255)]
    public required string Name { get; set; }
}