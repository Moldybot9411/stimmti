using System.ComponentModel.DataAnnotations;
using Backend.Models.Enums;

namespace Backend.Models;

public class AnonymousProfilePicture
{
    [Key]
    public Guid Id { get; set; }

    public BodyProfileEnum Body { get; set; }
    public ColorProfileEnum Color { get; set; }
    public FaceProfileEnum Face { get; set; }
    public HatProfileEnum Hat { get; set; }

    public required Guid AnonymousUserId { get; set; }
    public AnonymousUser? AnonymousUser { get; set; }
}