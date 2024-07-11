namespace AMChat.Hubs.Common.Models.Result;

public enum ResultType
{
    Success,
    ValidationError,
    NotFoundError,
    UnauthorisedError,
    ForbiddenError,
    ConflictError,
    InternalError
}
