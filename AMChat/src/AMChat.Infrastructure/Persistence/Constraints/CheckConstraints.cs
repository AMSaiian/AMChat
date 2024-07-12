using AMChat.Core;

namespace AMChat.Infrastructure.Persistence.Constraints;

public record CheckConstraint(string Name, string SqlCode);

public static class CheckConstraints
{
    public static readonly CheckConstraint MessagePostedDateNotInFuture =
        new("chk_messages_posted_time_not_in_future",
            "posted_time <= CURRENT_TIMESTAMP");

    public static readonly CheckConstraint ProfileBirthdateMoreThanMinimumAge =
        new("chk_messages_posted_time_not_in_future",
            $"AGE(CURRENT_DATE, birthdate) >= INTERVAL '{DomainConstraints.MinimalUserAge} years'");
}
