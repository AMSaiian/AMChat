using AMChat.Core.Entities;
using Bogus;

namespace AMChat.Infrastructure.Persistence.Seeding.Fakers;

public sealed class MessageFaker : Faker<Message>
{
    public MessageFaker()
    {
        RuleFor(message => message.Text,
                faker => faker.Lorem.Sentence());

        RuleFor(message => message.PostedTime,
                faker => faker.Date
                    .Recent(20)
                    .ToUniversalTime());
    }
}
