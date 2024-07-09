using System.Collections.ObjectModel;
using System.Linq.Expressions;
using AMChat.Core.Interfaces;

namespace AMChat.Core.Entities;

public class User : BaseEntity, IOrdered
{
    public static ReadOnlyDictionary<string, dynamic> OrderedBy { get; } = new(
        new Dictionary<string, dynamic>
        {
            { "username", (Expression<Func<User, string>>)(ent => ent.Name) },
            { "fullname", (Expression<Func<User, string>>)(ent => ent.Profile.Fullname) },
            { "birthdate", (Expression<Func<User, DateOnly>>)(ent => ent.Profile.Birthdate) }
        });

    public string Name { get; set; } = default!;

    public Profile Profile { get; set; } = default!;

    public List<Chat> OwnedChats { get; set; } = default!;

    public List<Chat> JoinedChats { get; set; } = default!;

    public List<Message> Messages { get; set; } = default!;
}
