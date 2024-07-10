using AMChat.Core.Entities;
using Bogus;

namespace AMChat.Infrastructure.Persistence.Seeding.Fakers;

public sealed class UserFaker : Faker<User>
{
    public UserFaker()
    {
        RuleFor(user => user.Name,
                faker => faker.Internet.UserName() + Guid.NewGuid());

        RuleFor(user => user.Messages,
                _ => []);

        RuleFor(user => user.JoinedChats,
                _ => []);

        RuleFor(user => user.OwnedChats,
                _ => []);
    }
}
