namespace AMChat.Core.Entities;

public class Message : BaseEntity
{
    public string Text { get; set; } = default!;

    public DateTime PostedTime { get; set; } = DateTime.UtcNow;

    public Guid AuthorId { get; set; }

    public Guid ChatId { get; set; }

    public User Author { get; set; } = default!;

    public Chat Chat { get; set; } = default!;
}
