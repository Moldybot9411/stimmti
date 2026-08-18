using System.ComponentModel.DataAnnotations;

public class UserUsernameAvailabilityResponseDto
{
    public bool IsAvailable { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class UserUsernameCheckRequestDto
{
    [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Username can only contain letters and digits")]
    [MaxLength(20, ErrorMessage = "Username is too long (max. 20 characters)")]
    public required string Username { get; set; }
}

public class DisplaynameCheckDto
{
    [RegularExpression("^[A-Za-z0-9]+$", ErrorMessage = "Displayname can only contain letters and digits")]
    [MaxLength(20, ErrorMessage = "Username is too long (max. 20 characters)")]
    public required string DisplayName { get; set; }
}