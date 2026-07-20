using System.ComponentModel.DataAnnotations;

public class UpdateFolderDto
{
    [MaxLength(255)]
    public string? Name { get; set; }

    
}