using System.Collections.ObjectModel;
using System.Linq.Expressions;
using AMChat.Core.Interfaces;

namespace AMChat.Core.Entities;

public class Chat : BaseEntity, IOrdered
{
    public static ReadOnlyDictionary<string, dynamic> OrderedBy { get; } = new(
        new Dictionary<string, dynamic>
        {
            { "name", (Expression<Func<Chat, string>>)(ent => ent.Name) },
            { "ownerName", (Expression<Func<Chat, string>>)(ent => ent.Owner.Name) }
        });

    public string Name { get; set; } = default!;

    public string? Description { get; set; }

    public Guid OwnerId { get; set; }

    public User Owner { get; set; } = default!;

    public List<User> JoinedUsers { get; set; } = default!;

    public List<Message> Messages { get; set; } = default!;
}
