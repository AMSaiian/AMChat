using AMChat.Core;

namespace AMChat.Application.Common.Templates;

public static class ErrorTemplates
{
    public static readonly string UniqueUsernameViolated =
        "User with defined username already exists";

    public static readonly string MinimumUserAgeViolated =
        $"User age must be greater than {DomainConstraints.MinimalUserAge}";

    public static readonly string EntityNotFoundFormat = "Entity {0} not found";
    public static readonly string EmptyUpdateObjectError = "No changes provided";

    public static readonly string ForbiddenUpdateNotOwnedResourceFormat =
        "Can't update {0} which is not owned by current user";
    public static readonly string ForbiddenDeleteNotOwnedResourceFormat =
        "Can't delete {0} which is not owned by current user";
}
