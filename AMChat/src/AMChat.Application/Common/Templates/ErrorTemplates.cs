using AMChat.Core;

namespace AMChat.Application.Common.Templates;

public static class ErrorTemplates
{
    public static readonly string ForbiddenUpdateNotOwnedUser =
        "Can't update user account which is not owned by user";
    public static readonly string UniqueUsernameViolated =
        "User with defined username already exists";

    public static readonly string MinimumUserAgeViolated =
        $"User age must be greater than {DomainConstraints.MinimalUserAge}";

    public static readonly string EntityNotFoundFormat = "Entity {0} not found";
    public static readonly string EmptyUpdateObjectError = "No changes provided";
}
