using AMChat.Core.Entities;
using Bogus;

namespace AMChat.Infrastructure.Persistence.Seeding.Fakers;

public sealed class ChatFaker : Faker<Chat>
{
    public ChatFaker()
    {
        RuleFor(chat => chat.Name,
                faker => faker.Lorem.Slug(5));

        RuleFor(chat => chat.Description,
                faker => faker.Lorem
                    .Paragraph(5)
                    .OrNull(faker, 0.3f));

        RuleFor(chat => chat.Messages,
                _ => []);

        RuleFor(chat => chat.JoinedUsers,
                _ => []);
    }
}
