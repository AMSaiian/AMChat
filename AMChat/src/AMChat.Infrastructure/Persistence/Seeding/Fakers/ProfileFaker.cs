﻿using AMChat.Core.Entities;
using AMChat.Infrastructure.Persistence.Constraints;
using Bogus;

namespace AMChat.Infrastructure.Persistence.Seeding.Fakers;

public sealed class ProfileFaker : Faker<Profile>
{
    public ProfileFaker()
    {
        RuleFor(profile => profile.Fullname,
                faker => faker.Person.FullName);

        RuleFor(profile => profile.Birthdate,
                faker => faker.Date.PastDateOnly(50,
                                                 DateOnly
                                                     .FromDateTime(DateTime.UtcNow)
                                                     .AddYears(-DataSchemeConstraints.MinimalUserAge)));

        RuleFor(profile => profile.Description,
                faker => faker.Lorem
                    .Paragraph(10)
                    .OrNull(faker, 0.20f));
    }
}
