namespace AMChat.Core.Entities;

public class Profile : BaseEntity
{
    public string Fullname { get; set; } = default!;

    public DateOnly Birthdate { get; set; }

    public string? Description { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = default!;
}
